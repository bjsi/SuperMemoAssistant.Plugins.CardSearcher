using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{

  using FieldHandlerFunc = Action<Dictionary<string, RenderContent>, string>;

  public partial class Renderer
  {

    public static string GetFieldName(string key) => key?.Split(':')?.LastOrDefault();

    // Special Fields
    public readonly HashSet<string> SpecialFields = new HashSet<string>
    {
      "Tags",
      "Type",
      "Deck",
      "Subdeck",
      "Card",
      "FrontSide"
    };

    private bool HandleFieldArgumentsAreValid(Dictionary<string, string> input, string key)
    {

      if (input.IsNull())
        return false;

      if (key.IsNullOrEmpty())
        return false;

      return true;

    }

    public void HandleField(Dictionary<string, string> obj, string key, TemplateType type)
    {

      if (!HandleFieldArgumentsAreValid(obj, key))
        return;

      string fieldName = GetFieldName(key);
      if (fieldName.IsNullOrEmpty())
        return;

      if (SpecialFields.Contains(fieldName))
        HandleSpecialField(key, type);

      else
        HandleUserField(obj, key, type);

    }

    private void HandleUserField(Dictionary<string, string> input, string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (!input.TryGetValue(fieldName, out var content))
      {
        LogTo.Error("Failed to HandleUnknownField");
        return;
      }

      var split = key.Split(':');

      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the fieldName
          .Reverse()
          .ToList();

        if (type == TemplateType.Question)
          ApplyQuestionFieldFilters(fieldName, filters);

        else
          ApplyAnswerFieldFilters(fieldName, filters);

      }
      else
      {

        if (type == TemplateType.Question)
        {

          QuestionContent[fieldName] = new RenderContent();
          QuestionContent[fieldName].Content = content;

        }
        else
        {

          AnswerContent[fieldName] = new RenderContent();
          AnswerContent[fieldName].Content = content;

        }

      }
    }

    private void HandleSpecialField(string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (fieldName == "Tags")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(TagFieldHandler, key);

        else
          HandleSpecialFieldQuestion(TagFieldHandler, key);

      }
      else if (fieldName == "Type")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(TypeFieldHandler, key);

        else
          HandleSpecialFieldQuestion(TypeFieldHandler, key);

      }
      else if (fieldName == "Deck")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(DeckFieldHandler, key);

        else
          HandleSpecialFieldQuestion(DeckFieldHandler, key);

      }
      else if (fieldName == "Subdeck")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(SubdeckFieldHandler, key);

        else
          HandleSpecialFieldQuestion(SubdeckFieldHandler, key);

      }
      else if (fieldName == "Card")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(CardFieldHandler, key);

        else
          HandleSpecialFieldQuestion(CardFieldHandler, key);

      }
      else if (fieldName == "FrontSide")
      {

        if (type == TemplateType.Answer)
          HandleSpecialFieldAnswer(FrontSideFieldHandler, key);

        else
          HandleSpecialFieldQuestion(FrontSideFieldHandler, key);
      }

    }

    private void HandleSpecialFieldAnswer(FieldHandlerFunc fieldHandler, string key)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      // Add the content from the special field handler
      fieldHandler(AnswerContent, fieldName);

      // Add filters if specified
      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        ApplyAnswerFieldFilters(fieldName, filters);

      }
    }

    private void HandleSpecialFieldQuestion(FieldHandlerFunc fieldHandler, string key)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      fieldHandler(QuestionContent, fieldName);

      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        ApplyQuestionFieldFilters(fieldName, filters);

      }
    }

    public void TagFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.Tags;
    }

    public void TypeFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.NoteType.Name;
    }

    public void SubdeckFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.NoteType.Name;
    }

    public void DeckFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.NoteType.Name;
    }

    public void CardFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.NoteType.Name;
    }

    public void FrontSideFieldHandler(Dictionary<string, RenderContent> input, string fieldName)
    {
      input[fieldName].Content = Card.Note.NoteType.Name;
    }
  }
}
