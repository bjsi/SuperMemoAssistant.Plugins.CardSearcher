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

  public class FieldRenderOptions
  {
    // Extract pictures from fields into their own components or leave them
    public bool ExtractPictures = true;
  }

  public partial class Renderer
  {

    // (1: cloze number), (2: text), (3: hint)
    private static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");

    // The card to be rendered.
    private Card Card { get; }

    // Rendered Field Content
    // Still contains media
    private Dictionary<string, string> AnswerContent { get; set; } = new Dictionary<string, string>();
    private Dictionary<string, string> QuestionContent { get; set; } = new Dictionary<string, string>();

    public Renderer(Card Card)
    {
      this.Card = Card;
    }

    private StubbleVisitorRenderer BuildRenderer(TemplateType type)
    {

      // Require a special value getter because  of anki's custom mustache templating.
      // The fieldContentMap is a mapping between field names and content,
      // but the template variables contain other information such as whether
      // the field should be formatted like a cloze, whether there is a hint and so on.

      return new StubbleBuilder()
      .Configure(settings =>
      {

        // stubble parses the anki card template parsing the strings between double braces {{ }}

        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {

          // The actual name of the field always comes last
          // Filters like cloze and hint are prepended to the field name, separated by colons :

          var input = val as Dictionary<string, string>;
          return type == TemplateType.Question
          ? HandleField(input, key, type) ?? string.Empty
          : HandleField(input, key, type) ?? string.Empty;

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();

    }

    /// <summary>
    /// Create the stubble html render for card content.
    /// </summary>
    /// <returns>Renderer or Null</returns>
    public async Task<Dictionary<string, string>> Render(TemplateType type)
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

      // Discards the string output, only focuses on creating the Question/AnswerContent fields
      if (type == TemplateType.Question)
      {

        await renderer.RenderAsync(Card.Template.QuestionFormat, Card.Note.Fields);
        return QuestionContent;

      }
      else
      {

        await renderer.RenderAsync(Card.Template.AnswerFormat, Card.Note.Fields);
        return AnswerContent;

      }
    }
  }
}
