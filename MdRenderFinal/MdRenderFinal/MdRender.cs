using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;

namespace MdRenderFinal;

public class MdRender
{
    public static void Main(string[] args)
    {
    }

    public string RenderHtml(StringBuilder mdString)
    {
        var paragraphJoin = new List<string>();
        var paragraphsSplit = SplitTextIntoParagraphs(mdString.ToString());
        foreach (var paragraph in paragraphsSplit)
        {
            StringBuilder sb = new StringBuilder(paragraph);
            var result = FilterAndModifyTags(SearchForTags(sb));
            paragraphJoin.Add(ConvertMdToHtml(sb, result));
        }

        return JoinParagraphs(paragraphJoin);
    }
    

    private static List<string> SplitTextIntoParagraphs(string text)
    {
        var paragraphs = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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
                        secondTag.HtmlTag = $"/{secondTag.HtmlTag}";
                        secondTag.Convert = true;
                        break;
                    }
                    else if (openTagInAnotherWord && firstTag.HtmlTag == secondTag.HtmlTag &&
                             secondTag.PositionInWord == TagPositionInWord.End)
                    {
                        flagTag = true;
                        firstTag.HtmlTag = $"{firstTag.HtmlTag}";
                        secondTag.HtmlTag = $"/{secondTag.HtmlTag}";
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
                HtmlTag = "/<h1>",
            });
        }

        return tags;
    }



    private static string ConvertMdToHtml(StringBuilder mdString,List<Tag> tags)
    {
        var indexIfLastTagParagrah = tags.Count==0? 1 : tags[tags.Count - 1].MdTag == "#" ? 2 : 1;
        if (indexIfLastTagParagrah == 2)
        {
            mdString.Append("/<h1>");
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
    
}