using Anotar.Serilog;
using Stubble.Core;
using Stubble.Core.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
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

    private TemplateRenderOptions RenderOptions { get; set; }


    // Rendered Field Content
    // Still contains media
    // private Dictionary<string, RenderContent> AnswerContent { get; set; } = new Dictionary<string, RenderContent>();
    // private Dictionary<string, RenderContent> QuestionContent { get; set; } = new Dictionary<string, RenderContent>();

    public Renderer(Card Card)
    {
      this.Card = Card;
    }

    private StubbleVisitorRenderer BuildRenderer(TemplateType type)
    {

      return new StubbleBuilder()
      .Configure(settings =>
      {

        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {

          var input = val as Dictionary<string, string>;
          return HandleField(input, key, type);

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();

    }

    /// <summary>
    /// Create the stubble html render for card content.
    /// </summary>
    /// <returns>Renderer or Null</returns>
    public string Render(TemplateType type)
    {

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
        return renderer.Render(Card.Template.QuestionFormat, Card.Note.Fields);

      else
        return renderer.Render(Card.Template.AnswerFormat, Card.Note.Fields);

    }
  }
}
