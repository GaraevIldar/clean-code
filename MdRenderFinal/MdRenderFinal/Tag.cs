namespace MdRenderFinal;

public class Tag
{
    public bool Convert = false;
    public int IndexStart { get; set; }
    public string MdTag { get; set; }
    public string HtmlTag { get; set; }
    
    public TagPositionInWord PositionInWord { get; set; }
    public int IdWordWithTag { get; set; }
}