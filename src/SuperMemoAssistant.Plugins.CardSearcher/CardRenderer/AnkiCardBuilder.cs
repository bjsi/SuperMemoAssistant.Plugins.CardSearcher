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
    private RenderContent QuestionContent { get; set; }
    private RenderContent AnswerContent { get; set; }
    private TemplateRenderOptions RenderOptions { get; set; }

    private Dictionary<string, string> PlaceholderMap => new Dictionary<string, string>
    {

      { "${CardType}",            Card.CardType.Name() },
      { "${DeckName}",            Card.Deck.Name },
      { "${SubdeckName}",         Card.Deck.Basename },
      { "${NoteTypeName}",        Card.Note.NoteType.Name },

    };

    public AnkiCardBuilder(Card card, RenderContent questionContent, RenderContent answerContent)
    {

      this.Card = card;
      this.QuestionContent = questionContent;
      this.AnswerContent = answerContent;

      AddFieldsToPlaceholderMap();

    }

    public AnkiCardBuilder(Card card,
                           RenderContent questionContent,
                           RenderContent answerContent,
                           TemplateRenderOptions renderOptions)
    {

      this.Card = card;
      this.QuestionContent = questionContent;
      this.AnswerContent = answerContent;
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

    public string RenderQuestionForImport()
    {

      return new StubbleBuilder()
      .Configure(settings =>
      {
        settings.AddValueGetter(typeof(Dictionary<string, RenderContent>), (obj, key, ignoreCase) =>
        {

          var input = obj as Dictionary<string, RenderContent>;
          var fieldName = key.Split(':').Last();
          return input[fieldName].Content;

        });
      })
      .Build()
      .Render(Card.Template.QuestionFormat, QuestionContent);

    }

    public string RenderAnswerForForImport()
    {

      return new StubbleBuilder()
      .Configure(settings =>
      {
        settings.AddValueGetter(typeof(Dictionary<string, RenderContent>), (obj, key, ignoreCase) =>
        {

          var input = obj as Dictionary<string, RenderContent>;
          var fieldName = key.Split(':').Last();
          return input[fieldName].Content;

        });
      })
      .Build()
      .Render(Card.Template.QuestionFormat, AnswerContent);

    }

    public async Task<ElementBuilder> CreateElementBuilder()
    {

      var contents = new List<ContentBase>();
      References refs = RenderOptions.Refs;
      refs.ReplacePlaceholders(PlaceholderMap);

      // Create Question Content
      string renderedQuestion = RenderQuestionForImport();
      if (renderedQuestion.IsNullOrEmpty())
      {

        LogTo.Warning("Failed to CreateElementBuilder, renderedQuestion was null or empty");
        return null;

      }

      if (RenderOptions.AddImageComponents)
      {

        var imgs = MediaParser.ParseImages(renderedQuestion);
        MediaParser.RemoveImageTags(renderedQuestion);

        foreach (var img in imgs)
        {

        }

      }

      // Create Answer Content
      string renderedAnswer = RenderAnswerForForImport();
      if (renderedAnswer.IsNullOrEmpty())
      {

        LogTo.Warning("Failed to CreateElementBuilder, renderedAnswer was null or empty");
        return null;

      }

      if (RenderOptions.AddImageComponents)
      {

        var imgs = MediaParser.ParseImages(renderedAnswer);
        MediaParser.RemoveImageTags(renderedAnswer);

        foreach (var img in imgs)
        {
        }

      }

      contents.Add(new TextContent(true, renderedAnswer, AtFlags.NonQuestion));
      contents.Add(new TextContent(true, renderedQuestion));

      return new ElementBuilder(
        ElementType.Item,
        contents.ToArray()
      )
      .WithReference(_ => refs);

    }
  }
}
