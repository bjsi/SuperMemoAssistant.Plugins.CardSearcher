using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher
{
  public static class MediaUtils
  {

    public static string ConvRelToAbsLink(string baseUrl, string relUrl)
    {
      if (!string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(relUrl))
      {
        if (baseUrl.EndsWith("\\"))
        {
          baseUrl = baseUrl.TrimEnd('\\');
        }

        return $"{baseUrl}\\{relUrl}";

      }
      return relUrl;
    }

    public static string FixMediaPaths(this string content, string mediaPath)
    {

      var doc = new HtmlDocument();
      doc.LoadHtml(content);
      var imgs = doc.DocumentNode.SelectNodes("//img");
      if (imgs.IsNull() || !imgs.Any())
        return content;

      foreach (var img in imgs)
      {

        string src = img.GetAttributeValue("src", null);
        if (src.IsNullOrEmpty())
          continue;

        string abs = ConvRelToAbsLink(mediaPath, src);
        if (abs.IsNullOrEmpty())
          continue;

        img.SetAttributeValue("src", abs);

      }

      return doc.DocumentNode.OuterHtml;

    }

  }
}
