using Anotar.Serilog;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Interop.SuperMemo.Content.Models;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
      this.RenderOptions = new TemplateRenderOptions();
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

    private ImageContent NewImageContent(string src, TemplateType type)
    {

      AtFlags displayAt = type == TemplateType.Question
        ? AtFlags.All
        : AtFlags.NonQuestion;

      if (!File.Exists(src))
        return null;

      byte[] imageArray = File.ReadAllBytes(src);
      if (imageArray.IsNull() || !imageArray.Any())
        return null;

      using (MemoryStream ms = new MemoryStream(imageArray))
      {

        Image image = Image.FromStream(ms);

        // Add to registry
        int regId = Svc.SM.Registry.Image.Add(new ImageWrapper(image), src);
        if (regId == -1)
          return null;

        return new ImageContent(regId, displayAt: displayAt);
      }

    }

    private string ParseAndAddImages(string content, List<ContentBase> contents, TemplateType type)
    {

      var imgs = MediaParser.ParseImages(content);
      content = MediaParser.RemoveImageTags(content);

      if (imgs.IsNull() || !imgs.Any())
        return content;

      foreach (var src in imgs) { contents.Add(NewImageContent(src, type)); }

      return content;

    }

    // TODO:
    private string ParseAndAddSound(string content, List<ContentBase> contents, TemplateType type)
    {

      return MediaParser.RemoveAudioTags(content);

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

        question = ParseAndAddImages(question, contents, TemplateType.Question);
        answer = ParseAndAddImages(answer, contents, TemplateType.Answer);

      }

      // Audio
      question = ParseAndAddSound(question, contents, TemplateType.Question);
      answer = ParseAndAddSound(answer, contents, TemplateType.Answer);

      // Video
           

      // Add Content
      contents.Add(new TextContent(true, question));
      contents.Add(new TextContent(true, answer, AtFlags.NonQuestion));

      // Create Element Builder
      return new ElementBuilder(
        ElementType.Item,
        contents.ToArray()
      )
      .WithLayout(RenderOptions.Layout)
      .DoNotDisplay()
      .WithReference(_ => refs);

    }
  }
}
