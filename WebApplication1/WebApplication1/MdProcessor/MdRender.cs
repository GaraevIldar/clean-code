
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MdRenderFinal;

public class MdRender
{

    public string RenderHtml(StringBuilder markdownText)
    {
        var paragraphs = SplitTextIntoParagraphs(markdownText.ToString());
        var processedParagraphs = ProcessParagraphs(paragraphs);

        var markdownListHtml = ConvertMarkdownListToHtml(processedParagraphs);
        var finalParagraphs = SplitTextIntoParagraphs(markdownListHtml);

        return JoinParagraphs(finalParagraphs);
    }

    private List<string> ProcessParagraphs(List<string> paragraphs)
    {
        var result = new List<string>();
        foreach (var paragraph in paragraphs)
        {
            var sb = new StringBuilder(paragraph);
            var tags = SearchForTags(sb);
            var modifiedTags = FilterAndModifyTags(tags);
            result.Add(ConvertMdToHtml(sb, modifiedTags));
        }
        return result;
    }
    

    private static List<string> SplitTextIntoParagraphs(string text)
    {
        var paragraphs = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); 
        return paragraphs.ToList();
    }

    private static string JoinParagraphs(List<string> paragraphs)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < paragraphs.Count; i++)
        {
            sb.Append(paragraphs[i]); 
            if (i < paragraphs.Count - 1) 
            {
                sb.AppendLine(); 
            }
        }

        return sb.ToString(); 
    }

    private List<Tag> SearchForTags(StringBuilder mdString)
    {
        List<Tag> TagsRenderInHtml = new List<Tag>();
        int idWordWithTag = 0;
        bool inWord = false; 
        int wordStartIndex = -1;
        for (int i = 0; i < mdString.Length; i++)
        {
            char currentChar = mdString[i];
            if (currentChar == '\\')
            {
                if (i + 1 < mdString.Length)
                {
                    char nextChar = mdString[i + 1];
                    if (nextChar == '_')
                    {
                        if(i + 2 < mdString.Length && mdString[i + 2] == '_')
                        {
                            mdString.Remove(i, 1);
                            i++;
                            continue;
                        }
                        else
                        {
                            mdString.Remove(i, 1);
                            continue;
                        }
                    }

                    if (nextChar == '\\')
                    {
                        mdString.Remove(i, 1);
                        continue;
                    }
                    
                }
            }
            
            if (currentChar == ' ')
            {
                inWord = false;
                idWordWithTag++;
                continue;
            }

            if (!inWord)
            {
                inWord = true;
                wordStartIndex = i;
            }

            if (currentChar == '#')
            {
                TagsRenderInHtml.Add(TagCreation("#", i, TagPositionInWord.Start,idWordWithTag));
            }
            if (i < mdString.Length - 1 && currentChar == '_' && mdString[i + 1] == '_')
            {
                bool isPrecededByDigit = (i > 0 && char.IsDigit(mdString[i - 1]));
                bool isFollowedByDigit = (i + 2 < mdString.Length && char.IsDigit(mdString[i + 2]));

                if (!isPrecededByDigit && !isFollowedByDigit) 
                {
                    if (i == wordStartIndex)
                    {
                        TagsRenderInHtml.Add(TagCreation("__", i, TagPositionInWord.Start, idWordWithTag));
                    }
                    else if (i + 2 >= mdString.Length || mdString[i + 2] == ' ')
                    {
                        TagsRenderInHtml.Add(TagCreation("__", i, TagPositionInWord.End, idWordWithTag));
                    }
                    else
                    {
                        TagsRenderInHtml.Add(TagCreation("__", i, TagPositionInWord.Center, idWordWithTag));
                    }
                }

                i++; 
            }

            else if (currentChar == '_')
            {
                bool isPrecededByDigit = (i > 0 && char.IsDigit(mdString[i - 1]));
                bool isFollowedByDigit = (i + 1 < mdString.Length && char.IsDigit(mdString[i + 1]));

                if (!isPrecededByDigit && !isFollowedByDigit) 
                {
                    if (i == wordStartIndex)
                    {
                        TagsRenderInHtml.Add(TagCreation("_", i, TagPositionInWord.Start, idWordWithTag));
                    }
                    else if (i == mdString.Length - 1 || (i + 1 < mdString.Length && mdString[i + 1] == ' '))
                    {
                        TagsRenderInHtml.Add(TagCreation("_", i, TagPositionInWord.End, idWordWithTag));
                    }
                    else
                    {
                        TagsRenderInHtml.Add(TagCreation("_", i, TagPositionInWord.Center, idWordWithTag));
                    }
                }
            }
        }
        
        return TagsRenderInHtml;
    }


    private static List<Tag> FilterAndModifyTags(List<Tag> tags)
    {
        int indexIfStartParagrah = tags.Count == 0 ? 0 : tags[0].MdTag == "#" ? 1 : 0;

        for (int i = indexIfStartParagrah; i < tags.Count; i++)
        {
            bool openTagInAnotherWord = (i + 1 < tags.Count && tags[i + 1].IdWordWithTag != tags[i].IdWordWithTag && tags[i].PositionInWord == TagPositionInWord.Start);
            bool flagTag = false;
            var firstTag = tags[i];
            if (i > 0 && i < tags.Count - 1)
            {
                if (tags[i + 1].MdTag == tags[i - 1].MdTag && tags[i - 1].MdTag != firstTag.MdTag)
                {
                    tags[i + 1].HtmlTag = tags[i + 1].MdTag;
                    tags[i - 1].HtmlTag = tags[i - 1].MdTag;
                    firstTag.HtmlTag = firstTag.MdTag;
                    flagTag = true;
                }
            }
            if (i > 1 && i < tags.Count - 1)
            {
                if (tags[i + 1].MdTag == tags[i - 2].MdTag && tags[i - 1].MdTag == firstTag.MdTag && tags[i - 1].MdTag != tags[i - 2].MdTag && tags[i - 1].MdTag=="__" && tags[i - 2].MdTag=="_")
                {
                    tags[i - 1].HtmlTag = tags[i - 1].MdTag;
                    firstTag.HtmlTag = firstTag.MdTag;
                    flagTag = true;
                }
            }
            if(flagTag == false)
            {
                for (int j = i + 1; j < tags.Count; j++)
                {
                    var secondTag = tags[j];

                    if (firstTag.IdWordWithTag == secondTag.IdWordWithTag && firstTag.HtmlTag == secondTag.HtmlTag)
                    {
                        flagTag = true;
                        firstTag.HtmlTag = $"{firstTag.HtmlTag}";
                        secondTag.HtmlTag = $"</{secondTag.HtmlTag.Substring(1)}";
                        secondTag.Convert = true;
                        break;
                    }
                    else if (openTagInAnotherWord && firstTag.HtmlTag == secondTag.HtmlTag &&
                             secondTag.PositionInWord == TagPositionInWord.End)
                    {
                        flagTag = true;
                        firstTag.HtmlTag = $"{firstTag.HtmlTag}";
                        secondTag.HtmlTag = $"</{secondTag.HtmlTag.Substring(1)}";
                        secondTag.Convert = true;
                        break;
                    }
                }
            }
            if(flagTag == false && firstTag.Convert==false){firstTag.HtmlTag = firstTag.MdTag;}
        }
        
        if (indexIfStartParagrah == 1)
        {
            tags.Add(new Tag()
            {
                MdTag = "#",
                HtmlTag = "</h1>",
            });
        }

        return tags;
    }



    private static string ConvertMdToHtml(StringBuilder mdString,List<Tag> tags)
    {
        var indexIfLastTagParagrah = tags.Count==0? 1 : tags[tags.Count - 1].MdTag == "#" ? 2 : 1;
        if (indexIfLastTagParagrah == 2)
        {
            mdString.Append("</h1>");
        }
        for(int i = tags.Count - indexIfLastTagParagrah; i >= 0; i--)
        {
            var tag = tags[i];
            mdString.Replace(tag.MdTag, tag.HtmlTag, tag.IndexStart,tag.MdTag.Length);
        }
        return mdString.ToString();
    }

    private static Tag TagCreation(string tag,int index,TagPositionInWord tagPositionInWord, int idWordWithTag)
    {
        return  new Tag()
        {
            MdTag = tag,
            HtmlTag = MdTagToHtmlForTagCreation(tag),
            IndexStart = index,
            PositionInWord = tagPositionInWord,
            IdWordWithTag = idWordWithTag,
        };
    }

    private static string MdTagToHtmlForTagCreation(string tag)
    {
        switch (tag)
        {
            case "_":
                return "<em>"; 
                break;
            case "__":
                return "<strong>";
                break;
            case "#":
                return "<h1>";
                break;
        }

        return null;
    }
    
    
    public string ConvertMarkdownListToHtml(List<string> markdownLines)
    {
        StringBuilder html = new StringBuilder();
        bool insideList = false;

        for (int i = 0; i < markdownLines.Count; i++)
        {
            int leadingSpaces = GetLeadingSpaces(markdownLines[i]);
            string trimmedLine = markdownLines[i].Trim();
            
            if (trimmedLine.StartsWith("-"))
            {
                if (!insideList)
                {
                    html.Append("<ul>\n");
                    insideList = true;
                }
                html.Append(new string(' ', leadingSpaces + 2)); 
                html.Append($"<li>{trimmedLine.Substring(1).Trim()}</li>\n");
            }
            else
            {
                if (insideList)
                {
                    html.Append("</ul>\n");
                    insideList = false;
                }
                
                html.Append($"{trimmedLine}\n");
            }
        }
        
        if (insideList)
        {
            html.Append("</ul>\n");
        }

        return html.ToString();
    }

    private int GetLeadingSpaces(string line)
    {
        int count = 0;
        while (count < line.Length && line[count] == ' ')
        {
            count++;
        }
        return count;
    }
    
}