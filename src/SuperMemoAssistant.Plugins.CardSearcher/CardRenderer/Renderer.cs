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

  public class RenderContent
  {

    public string Content { get; set; }
    public List<string> AppliedFilters = new List<string>();
    public List<string> UnappliedFilters = new List<string>();

  }

  public partial class Renderer
  {

    // (1: cloze number), (2: text), (3: hint)
    private static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");
    
    // The card to be rendered.
    private Card Card { get; }

    private string CollectionPath { get; set; }
    private string MediaPath { get; set; }

    private Dictionary<string, string> RenderedFields { get; set; } = new Dictionary<string, string>();

    public Renderer(Card Card)
    {

      this.Card = Card;
      var Config = Svc<CardSearcherPlugin>.Plugin.Config;
      CollectionPath = Config.AnkiCollectionPath;
      MediaPath = Config.AnkiMediaPath;
      

    }

    // For test cases
    public Renderer(Card card, string collectionPath, string mediaPath)
    {

      this.Card = card;
      this.CollectionPath = collectionPath;
      this.MediaPath = mediaPath;

    }

    private StubbleVisitorRenderer BuildRenderer(TemplateType type)
    {

      return new StubbleBuilder()
      .Configure(settings =>
      {

        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {

          var input = val as Dictionary<string, string>;
          string output = HandleField(input, key, type);
          string fieldName = GetFieldName(key);
          RenderedFields[fieldName] = output;
          return output;

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();

    }


    /// <summary>
    /// Create the stubble html render for card content.
    /// </summary>
    /// <returns>Renderer or Null</returns>
    public string Render(TemplateType type, out Dictionary<string, string> renderedFieldsDict)
    {

      renderedFieldsDict = new Dictionary<string, string>();

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

        renderedFieldsDict = RenderedFields;
        string output = renderer.Render(Card.Template.QuestionFormat, Card.Note.Fields);
        return output
          .FixMediaPaths(MediaPath)
          .AddCss(Card.Note.NoteType.CSS)
          .ReplaceWhitespace();

      }
      else
      {

        renderedFieldsDict = RenderedFields;
        string output = renderer.Render(Card.Template.AnswerFormat, Card.Note.Fields);
        return output
          .FixMediaPaths(MediaPath)
          .AddCss(Card.Note.NoteType.CSS)
          .ReplaceWhitespace();

      }

    }
  }
}
