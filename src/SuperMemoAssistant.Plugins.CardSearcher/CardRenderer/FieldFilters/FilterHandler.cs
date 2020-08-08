using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class FilterHandler
  {

    private static HashSet<string> BuiltInFilters = new HashSet<string>
    {

      "hint",
      "type",
      "cloze",
      "text"

    };

    public static string ApplyQuestionFieldFilters(List<string> filters, string input, Card card)
    {

      if (filters.IsNull() || !filters.Any())
        return input;

      if (input.IsNullOrEmpty())
        return input;

      foreach (var filter in filters)
      {

        if (!BuiltInFilters.Contains(filter))
        {
          LogTo.Warning($"FilterHandler failed to handle unrecognised filter {filter}");
          continue;
        }

        if (filter == "hint")
          input = SharedFilters.HintFilter(input);

        else if (filter == "type")
          input = QuestionFilters.QuestionTypeFilter(input);

        else if (filter == "cloze")
          input = QuestionFilters.QuestionClozeFilter(input, card);

        else if (filter == "text")
          input = SharedFilters.TextFilter(input);

      }

      return input;

    }

    public static string ApplyAnswerFieldFilters(List<string> filters, string input, Card card)
    {

      if (filters.IsNull() || !filters.Any())
        return input;

      if (input.IsNullOrEmpty())
        return input;

      foreach (var filter in filters)
      {

        if (!BuiltInFilters.Contains(filter))
        {
          LogTo.Warning($"FilterHandler failed to handle unrecognised filter {filter}");
          continue;
        }

        if (filter == "hint")
          input = SharedFilters.HintFilter(input);

        else if (filter == "type")
          input = AnswerFilters.AnswerTypeFilter(input);

        else if (filter == "cloze")
          input = AnswerFilters.AnswerClozeFilter(input, card);

        else if (filter == "text")
          input = SharedFilters.TextFilter(input);

      }

      return input;

    }
  }
}
