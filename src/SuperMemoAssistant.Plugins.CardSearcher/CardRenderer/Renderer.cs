using Anotar.Serilog;
using HtmlAgilityPack;
using Stubble.Core;
using Stubble.Core.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public enum TemplateType
  {
    Question,
    Answer
  }

  public class RenderOptions
  {

    /// <summary>
    /// Fields that appear on the question side won't be duplicated on the answer side
    /// </summary>
    public bool IgnoreDuplicateFields { get; set; }

    /// <summary>
    /// If ignore duplicate fields is true, this must be passed to evaluate which 
    /// fields have already been rendered.
    /// </summary>
    public Dictionary<string, string> RenderedQuestionFields { get; set; }

  }

  public partial class Renderer
  {

    // (1: cloze number), (2: text), (3: hint)
    private static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");
    
    /// <summary>
    /// The card to be rendered
    /// </summary>
    private Card Card { get; }

    /// <summary>
    /// Anki collection database path
    /// </summary>
    private string CollectionPath { get; set; }

    /// <summary>
    /// Anki collection media path
    /// </summary>
    private string MediaPath { get; set; }

    private Dictionary<string, string> RenderedFields { get; set; } = new Dictionary<string, string>();

    public Renderer(Card Card)
    {

      this.Card = Card;
      var Config = Svc<CardSearcherPlugin>.Plugin.Config;
      CollectionPath = Config.AnkiCollectionPath;
      MediaPath = Config.AnkiMediaPath;
      

    }

    /// <summary>
    /// For test cases
    /// </summary>
    /// <param name="card"></param>
    /// <param name="collectionPath"></param>
    /// <param name="mediaPath"></param>
    public Renderer(Card card, string collectionPath, string mediaPath)
    {

      this.Card = card;
      this.CollectionPath = collectionPath;
      this.MediaPath = mediaPath;

    }

    private StubbleVisitorRenderer BuildIgnoreDuplicatesAnswerRenderer(Dictionary<string, string> renderedQuestionFields)
    {

      return new StubbleBuilder()
        .Configure(settings =>
        {
          settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
          {

            var input = val as Dictionary<string, string>;
            string output = FieldHandler.HandleField(input, key, TemplateType.Answer, Card);
            string fieldName = FieldHandler.GetFieldName(key);

            // Only include field in answer if it is not already in the question
            if (!renderedQuestionFields.ContainsKey(fieldName))
            {
              RenderedFields[fieldName] = output;
              return output;
            }
            return string.Empty;

          })
          .SetEncodingFunction(x => x); // allow unescaped html
        })
        .Build();

    }

    private StubbleVisitorRenderer BuildRenderer(TemplateType type)
    {

      return new StubbleBuilder()
      .Configure(settings =>
      {

        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {

          var input = val as Dictionary<string, string>;
          string output = FieldHandler.HandleField(input, key, type, Card);
          string fieldName = FieldHandler.GetFieldName(key);
          RenderedFields[fieldName] = output;
          return output;

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();

    }

    public string RenderAnswerIgnoreDuplicates(Dictionary<string, string> renderedQuestionFields, out Dictionary<string, string> fieldDict)
    {

      fieldDict = new Dictionary<string, string>();

      if (this.Card.IsNull() || renderedQuestionFields.IsNull())
      {

        LogTo.Warning("Failed to Render because card was null");
        return null;

      }

      var renderer = BuildIgnoreDuplicatesAnswerRenderer(renderedQuestionFields);
      if (renderer.IsNull())
      {

        LogTo.Error("Failed to Render because the attempt to BuildRenderer returned null");
        return null;

      }

      fieldDict = RenderedFields;
      string output = renderer.Render(Card.Template.AnswerFormat, Card.Note.Fields);
      return output
        .FixMediaPaths(MediaPath)
        .AddCss(Card.Note.NoteType.CSS)
        .ReplaceWhitespace();

    }

    public string Render(TemplateType type, out Dictionary<string, string> fieldDict)
    {

      fieldDict = new Dictionary<string, string>();

      if (this.Card == null)
      {

        LogTo.Warning("Failed to Render because card was null");
        return null;

      }

      var renderer = BuildRenderer(type);
      if (renderer.IsNull())
      {

        LogTo.Error("Failed to Render because the attempt to BuildRenderer returned null");
        return null;

      }

      if (type == TemplateType.Question)
      {

        fieldDict = RenderedFields;
        string output = renderer.Render(Card.Template.QuestionFormat, Card.Note.Fields);
        return output
          .FixMediaPaths(MediaPath)
          .AddCss(Card.Note.NoteType.CSS)
          .ReplaceWhitespace();

      }
      else
      {

        fieldDict = RenderedFields;
        string output = renderer.Render(Card.Template.AnswerFormat, Card.Note.Fields);
        return output
          .FixMediaPaths(MediaPath)
          .AddCss(Card.Note.NoteType.CSS)
          .ReplaceWhitespace();

      }

    }
  }
}
