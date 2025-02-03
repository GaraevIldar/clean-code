using System.Text;

namespace MdRenderFinal
{
    public interface IMdRender
    {
        /// <summary>
        /// Преобразует переданный Markdown текст в HTML.
        /// </summary>
        /// <param name="markdownText">Входной Markdown текст.</param>
        /// <returns>Сгенерированный HTML.</returns>
        string RenderHtml(StringBuilder markdownText);
        
    }
}
