using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class MediaParser
  {

    // TODO: incomplete
    private static readonly string[] SupportedImageFormats = new[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };
    private static readonly string[] SupportedSoundFormats = new[] { ".wav", ".mp3", ".ogg" };
    private static readonly string[] SupportedVideoFormats = new[] { "" };

    public static List<string> ParseImages(string contentString)
    {
      List<string> relfps = new List<string>();

      if (string.IsNullOrEmpty(contentString))
        return relfps;

      var doc = new HtmlDocument();
      doc.LoadHtml(contentString);

      var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]");
      if (imgNodes == null)
        return relfps;

      foreach (var imgNode in imgNodes)
      {
        string fp = imgNode.GetAttributeValue("src", null);
        if (string.IsNullOrEmpty(fp))
          continue;

        string extension = Path.GetExtension(fp);

        if (SupportedImageFormats.Any(format => format == extension))
          relfps.Add(fp);
      }

      return relfps;
    }
    public static List<string> ParseAudioTags(string content)
    {

      var relfps = new List<string>();

      if (string.IsNullOrEmpty(content))
        return relfps;

      Match match = AnkiRegexes.AudioVideoRegex.Match(content);
      while (match.Success && match.Groups.Count >= 2)
      {
        string fp = match.Groups[1].Value;
        if (!string.IsNullOrEmpty(fp))
        {
          string extension = Path.GetExtension(fp);

          if (SupportedSoundFormats.Any(format => format == extension))
            relfps.Add(fp.Trim());
        }
        match = match.NextMatch();
      }

      return relfps;
    }

    public static string RemoveAudioTags(string content)
    {
      return Regex.Replace(content, AnkiRegexes.AudioVideoPattern, "");
    }

    public static List<string> ParseVideoTags(string contentString)
    {
      var relfps = new List<string>();

      if (string.IsNullOrEmpty(contentString))
        return relfps;

      Match match = AnkiRegexes.AudioVideoRegex.Match(contentString);
      while (match.Success && match.Groups.Count >= 2)
      {
        string filepath = match.Groups[1].Value;
        if (!string.IsNullOrEmpty(filepath))
        {
          var extension = Path.GetExtension(filepath);

          if (SupportedVideoFormats.Any(format => format == extension))
            relfps.Add(filepath);
        }
        match = match.NextMatch();
      }

      return relfps;
    }

    public static string RemoveAVTags(string contentString)
    {
      string cleanString = string.Empty;

      if (string.IsNullOrEmpty(contentString))
        return string.Empty;

      Match match = AnkiRegexes.AudioVideoRegex.Match(contentString);
      int prevIdx = 0;
      while (match.Success && match.Groups.Count >= 2)
      {
        cleanString += contentString.Substring(prevIdx, match.Index - prevIdx);
        prevIdx = match.Index + match.Length;
        match = match.NextMatch();
      }

      cleanString += contentString.Substring(prevIdx);
      return cleanString;
    }

    public static string RemoveImageTags(string contentString)
    {
      if (string.IsNullOrEmpty(contentString))
        return string.Empty;

      var doc = new HtmlDocument();
      doc.LoadHtml(contentString);

      var imgNodes = doc.DocumentNode.SelectNodes("//img");
      if (imgNodes.IsNull() || !imgNodes.Any())
        return contentString;

      foreach (var img in imgNodes)
        img.ParentNode.RemoveChild(img);

      return doc.DocumentNode.OuterHtml;
    }
  }
}
