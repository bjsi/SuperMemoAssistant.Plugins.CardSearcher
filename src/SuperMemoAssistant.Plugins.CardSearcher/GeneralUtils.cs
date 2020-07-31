using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher
{
  public static class GeneralUtils
  {

    public static string InnerText(this string text)
    {
      if (text.IsNullOrEmpty())
        return string.Empty;

      var doc = new HtmlDocument();
      doc.LoadHtml(text);
      return doc.DocumentNode.InnerText;

    }

    /// <summary>
    /// Determine whether the object is null
    /// </summary>
    /// <param name="obj"></param>
    public static bool IsNull(this object obj)
    {
      return obj == null;
    }

    /// <summary>
    /// Determine whether the string is null or empty
    /// </summary>
    /// <param name="str"></param>
    public static bool IsNullOrEmpty(this string str)
    {
      return string.IsNullOrEmpty(str);
    }
  }
}
