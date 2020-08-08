using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.CardRenderer.FieldHandlers;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{

  public static class FieldHandler
  {

    public static string GetFieldName(string key) => key?.Split(':')?.LastOrDefault();

    // Special Fields
    private static readonly HashSet<string> Fields = new HashSet<string>
    {

      "Tags",
      "Type",
      "Deck",
      "Subdeck",
      "Card",
      "FrontSide"

    };

    public static bool HandleFieldArgumentsAreValid(Dictionary<string, string> input, string key)
    {

      if (input.IsNull())
        return false;

      if (key.IsNullOrEmpty())
        return false;

      return true;

    }

    public static string HandleField(Dictionary<string, string> obj, string key, TemplateType type, Card card)
    {

      if (!HandleFieldArgumentsAreValid(obj, key))
        return string.Empty;

      string fieldName = GetFieldName(key);
      if (fieldName.IsNullOrEmpty())
        return string.Empty;

      if (Fields.Contains(fieldName))
       return HandleSpecialField(key, type, card);

      else
        return HandleUserField(obj, key, type, card);

    }

    private static string HandleUserField(Dictionary<string, string> input, string key, TemplateType type, Card card)
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
          fieldContent = FilterHandler.ApplyQuestionFieldFilters(filters, fieldContent, card);

        else
          fieldContent = FilterHandler.ApplyAnswerFieldFilters(filters, fieldContent, card);

      }

      return fieldContent;
    }

    private static string HandleSpecialField(string key, TemplateType type, Card card)
    {

      string fieldName = GetFieldName(key);

      if (fieldName == "Tags")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.TagFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.TagFieldHandler, key, card);

      }
      else if (fieldName == "Type")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.TypeFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.TypeFieldHandler, key, card); 

      }
      else if (fieldName == "Deck")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.DeckFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.DeckFieldHandler, key, card);

      }
      else if (fieldName == "Subdeck")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.SubdeckFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.SubdeckFieldHandler, key, card);

      }
      else if (fieldName == "Card")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.CardFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.CardFieldHandler, key, card);

      }
      else if (fieldName == "FrontSide")
      {

        return type == TemplateType.Answer
          ? HandleSpecialFieldAnswer(SpecialFields.FrontSideFieldHandler, key, card)
          : HandleSpecialFieldQuestion(SpecialFields.FrontSideFieldHandler, key, card);

      }
      else
      {
        return string.Empty;
      }

    }

    private static string HandleSpecialFieldAnswer(Func<Card, string> specialFieldFunc, string key, Card card)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      // Add the content from the special field handler
      string specialContent = specialFieldFunc(card);

      // Add filters if specified
      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        specialContent = FilterHandler.ApplyAnswerFieldFilters(filters, specialContent, card);

      }

      return specialContent;
    }

    private static string HandleSpecialFieldQuestion(Func<Card, string> specialFieldFunc, string key, Card card)
    {

      var split = key.Split(':');
      var fieldName = split.Last();

      string specialContent = specialFieldFunc(card);

      if (split.Length > 1)
      {

        // Has filters
        var filters = split
          .Skip(1) // Skip the field name
          .Reverse()
          .ToList();

        specialContent = FilterHandler.ApplyQuestionFieldFilters(filters, specialContent, card);

      }

      return specialContent;
    }
  }
}
