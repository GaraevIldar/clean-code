using System.Text;
using MdRenderFinal;

namespace WebApplication1.Services;

public class HelperService
{
    public string ProcessText(string input)
    {
        var md = new MdRender();
        return new string(md.RenderHtml(new StringBuilder(input)));
    }
}
