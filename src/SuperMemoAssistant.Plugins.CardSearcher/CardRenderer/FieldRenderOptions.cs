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

    /// <summary>
    /// Defines references for imported cards.
    /// Supports basic placeholder variables to add card-specific information.
    /// </summary>
    public References Refs { get; set; } = new References()
      .WithSource("Anki Deck \'${DeckName}\'")
      .WithDate(DateTime.Today)
      .WithTitle("Anki Card from \'${SubdeckName}\'");

    /// <summary>
    /// The layout for the imported card.
    /// </summary>
    public string Layout { get; set; }

    /// <summary>
    /// True if images should be extracted into their own components.
    /// </summary>
    public bool AddImageComponents { get; set; }

  }

  public static class ReferenceEx
  {

    public static void ReplacePlaceholders(this References refs, Dictionary<string, string> placeholderMap)
    {

      foreach (var kvpair in placeholderMap)
      {

        refs.Author = refs.Author?.Replace(kvpair.Key, kvpair.Value);
        refs.Comment = refs.Comment?.Replace(kvpair.Key, kvpair.Value);

        // Skip Dates

        refs.Email = refs.Comment?.Replace(kvpair.Key, kvpair.Value);
        refs.Link = refs.Link?.Replace(kvpair.Key, kvpair.Value);
        refs.Source = refs.Source?.Replace(kvpair.Key, kvpair.Value);
        refs.Title = refs.Title?.Replace(kvpair.Key, kvpair.Value);

      }
    }
  }
}
