using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{

  using FieldHandlerFunc = Func<Dictionary<string, string>, string, string>;

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

    public string HandleField(Dictionary<string, string> obj, string key, TemplateType type)
    {

      if (!HandleFieldArgumentsAreValid(obj, key))
        return string.Empty;

      string fieldName = GetFieldName(key);
      if (fieldName.IsNullOrEmpty())
        return string.Empty;

      if (SpecialFields.Contains(fieldName))
        return HandleSpecialField(obj, key, type);

      else
        return HandleUserField(obj, key, type);

    }

    private void HandleUserField(Dictionary<string, string> input, string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (!input.TryGetValue(fieldName, out var content))
      {
        LogTo.Error("Failed to HandleUnknownField");
        return;
      }

      if (key.Split(':').Length > 1)
      {

        // Has filters
        var filters = key
          .Split(':')
          .Skip(1)
          .Reverse()
          .ToList();

        if (type == TemplateType.Question)
          ApplyQuestionFieldFilters(fieldName, filters);
        else
          ApplyAnswerFieldFilters(fieldName, filters);

      }
    }

    private string HandleSpecialField(Dictionary<string, string> obj, string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (fieldName == "Tags")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(TagFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(TagFieldHandler, obj, key);

      else if (fieldName == "Type")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(TypeFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(TypeFieldHandler, obj, key);

      else if (fieldName == "Deck")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(DeckFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(DeckFieldHandler, obj, key);

      else if (fieldName == "Subdeck")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SubdeckFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(SubdeckFieldHandler, obj, key);

      else if (fieldName == "Card")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(CardFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(CardFieldHandler, obj, key);

      else if (fieldName == "FrontSide")
        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(FrontSideFieldHandler, obj, key)
          : HandleSpecialFieldQuestion(FrontSideFieldHandler, obj, key);

      return null;

    }

    private string HandleSpecialFieldAnswer(FieldHandlerFunc func, Dictionary<string, string> input, string key)
    {

      var split = key.Split(':');

      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        func(input, key);
        // TODO:

      }
      else
      {

        // Does not have filters
        return Card.Note.Tags;

      }
    }

    private string HandleSpecialFieldQuestion(FieldHandlerFunc func, Dictionary<string, string> input, string key)
    {

      if (key.Split(':').Length > 1)
      {

        // Has filters
        var filters = key
          .Split(':')
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

      }
      else
      {

        // Does not have filters
        return Card.Note.Tags;

      }

    }

    public string TagFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.Tags;
    }

    public string TypeFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.NoteType.Name;
    }

    public string SubdeckFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.NoteType.Name;
    }

    public string DeckFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.NoteType.Name;
    }

    public string CardFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.NoteType.Name;
    }

    public string FrontSideFieldHandler(Dictionary<string, string> input, string key)
    {
      return Card.Note.NoteType.Name;
    }
  }
}
