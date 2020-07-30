using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{

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
       return HandleSpecialField(key, type);

      else
        return HandleUserField(obj, key, type);

    }

    private string HandleUserField(Dictionary<string, string> input, string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (!input.TryGetValue(fieldName, out var fieldContent))
      {
        LogTo.Error("Failed to HandleUnknownField");
        return string.Empty;
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
          fieldContent = ApplyQuestionFieldFilters(filters, fieldContent);

        else
          fieldContent = ApplyAnswerFieldFilters(filters, fieldContent);

      }

      return fieldContent;
    }

    private string HandleSpecialField(string key, TemplateType type)
    {

      string fieldName = GetFieldName(key);

      if (fieldName == "Tags")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(TagFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(TagFieldHandler, key);

      }
      else if (fieldName == "Type")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(TypeFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(TypeFieldHandler, key);

      }
      else if (fieldName == "Deck")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(DeckFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(DeckFieldHandler, key);

      }
      else if (fieldName == "Subdeck")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(SubdeckFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(SubdeckFieldHandler, key);

      }
      else if (fieldName == "Card")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(CardFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(CardFieldHandler, key);

      }
      else if (fieldName == "FrontSide")
      {

        if (type == TemplateType.Answer)
          return HandleSpecialFieldAnswer(FrontSideFieldHandler, key);

        else
          return HandleSpecialFieldQuestion(FrontSideFieldHandler, key);
      }
      else
      {
        return string.Empty;
      }

    }

    private string HandleSpecialFieldAnswer(Func<string> specialFieldFunc, string key)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      // Add the content from the special field handler
      string specialContent = specialFieldFunc();

      // Add filters if specified
      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        specialContent = ApplyAnswerFieldFilters(filters, specialContent);

      }

      return specialContent;
    }

    private string HandleSpecialFieldQuestion(Func<string> specialFieldFunc, string key)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      string specialContent = specialFieldFunc();

      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        specialContent = ApplyQuestionFieldFilters(filters, specialContent);

      }

      return specialContent;
    }

    public string TagFieldHandler()
    {
      return Card.Note.Tags;
    }

    public string TypeFieldHandler()
    {
      return Card.Note.NoteType.Name;
    }

    public string SubdeckFieldHandler()
    {
      return Card.Note.NoteType.Name;
    }

    public string DeckFieldHandler()
    {
      return Card.Note.NoteType.Name;
    }

    public string CardFieldHandler()
    {
      return Card.Note.NoteType.Name;
    }

    public string FrontSideFieldHandler()
    {
      return Card.Note.NoteType.Name;
    }
  }
}
