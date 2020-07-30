using Anotar.Serilog;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public partial class Renderer
  {

    public static void HintFilter(Dictionary<string, RenderContent> obj, string fieldName)
    {

      if (!obj.TryGetValue(fieldName, out var content))
      {
        LogTo.Error($"Failed to apply HintFilter to field {fieldName} because the content dict does not contain a corresponding key");
        return;
      }

      content.UnappliedFilters.Add("hint");

    }

    /// <summary>
    /// Removes HTML Tags
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldName"></param>
    public static void TextFilter(Dictionary<string, RenderContent> obj, string fieldName)
    {

      if (!obj.TryGetValue(fieldName, out var renderedContent))
      {
        LogTo.Error($"Failed to apply TextFilter to field {fieldName} because the content dict does not contain a corresponding key");
        return;
      }

      string content = renderedContent.Content;
      var doc = new HtmlDocument();

      // Remove html tags
      doc.LoadHtml(content);

      renderedContent.Content = doc.DocumentNode.InnerText;
      renderedContent.AppliedFilters.Add("text");

    }
  }
}
