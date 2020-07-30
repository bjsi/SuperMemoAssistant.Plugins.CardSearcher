using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{

  public class TemplateRenderOptions
  {

    public References Refs { get; set; }
    public bool AddImageComponents { get; set; }

  }

  public static class ReferenceEx
  {

    public static void ReplacePlaceholders(this References refs, Dictionary<string, string> placeholderMap)
    {

      foreach (var kvpair in placeholderMap)
      {

        refs.Author = refs.Author.Replace(kvpair.Key, kvpair.Value);
        refs.Comment = refs.Comment.Replace(kvpair.Key, kvpair.Value);

        // Skip Dates

        refs.Email = refs.Comment.Replace(kvpair.Key, kvpair.Value);
        refs.Link = refs.Link.Replace(kvpair.Key, kvpair.Value);
        refs.Source = refs.Source.Replace(kvpair.Key, kvpair.Value);
        refs.Title = refs.Title.Replace(kvpair.Key, kvpair.Value);

      }
    }
  }
}
