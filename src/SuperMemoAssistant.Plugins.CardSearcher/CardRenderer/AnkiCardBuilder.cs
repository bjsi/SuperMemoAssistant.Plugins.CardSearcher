using Anotar.Serilog;
using Stubble.Core.Builders;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Interop.SuperMemo.Content.Models;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public class AnkiCardBuilder
  {

    private Card Card { get; set; }
    private TemplateRenderOptions RenderOptions { get; set; }

    private Dictionary<string, string> PlaceholderMap => new Dictionary<string, string>
    {

      { "${CardType}",            Card.CardType.Name() },
      { "${DeckName}",            Card.Deck.Name },
      { "${SubdeckName}",         Card.Deck.Basename },
      { "${NoteTypeName}",        Card.Note.NoteType.Name },

    };

    public AnkiCardBuilder(Card card)
    {

      this.Card = card;
      AddFieldsToPlaceholderMap();

    }

    public AnkiCardBuilder(Card card, TemplateRenderOptions renderOptions)
    {

      this.Card = card;
      this.RenderOptions = renderOptions;

      AddFieldsToPlaceholderMap();

    }

    private void AddFieldsToPlaceholderMap()
    {

      foreach (var kvpair in Card.Note.Fields)
      {
        PlaceholderMap.Add($"${{Field:{kvpair.Key}}}", kvpair.Value);
      }

    }

    public ElementBuilder CreateElementBuilder()
    {

      var contents = new List<ContentBase>();
      References refs = RenderOptions.Refs;
      refs.ReplacePlaceholders(PlaceholderMap);

      string question = Card.Question;
      string answer = Card.Answer;
      if (question.IsNullOrEmpty() || answer.IsNullOrEmpty())
      {

        LogTo.Warning("Failed to CreateElementBuilder, question or answer string was null or empty");
        return null;

      }

      if (RenderOptions.AddImageComponents)
      {

        var imgs = MediaParser.ParseImages(question);
        MediaParser.RemoveImageTags(question);

        foreach (var img in imgs)
        {

        }

      }

      if (RenderOptions.AddImageComponents)
      {

        var imgs = MediaParser.ParseImages(answer);
        MediaParser.RemoveImageTags(answer);

        foreach (var img in imgs)
        {
        }

      }

      contents.Add(new TextContent(true, question));
      contents.Add(new TextContent(true, answer, AtFlags.NonQuestion));

      return new ElementBuilder(
        ElementType.Item,
        contents.ToArray()
      )
      .WithLayout(RenderOptions.Layout)
      .WithReference(_ => refs);

    }
  }
}
