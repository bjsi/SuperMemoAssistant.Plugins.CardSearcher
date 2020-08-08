using Anotar.Serilog;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class SharedFilters
  {

    // TODO:
    public static string HintFilter(string input)
    {
      return input;
    }

    /// <summary>
    /// Removes HTML Tags
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldName"></param>
    public static string TextFilter(string input)
    {

      if (input.IsNullOrEmpty())
        return input;

      var doc = new HtmlDocument();
      doc.LoadHtml(input);
      return doc.DocumentNode.InnerText;

    }
  }
}
