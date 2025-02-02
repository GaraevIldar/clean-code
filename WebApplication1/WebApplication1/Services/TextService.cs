using System.Text;
using MdRenderFinal;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class TextService
    {
        public async Task<string> ProcessTextAsync(string input)
        {
            var md = new MdRender();
            return await Task.Run(() =>
            {
                return new string(md.RenderHtml(new StringBuilder(input)));
            });
        }
    }
}