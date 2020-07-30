using Anotar.Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public partial class Renderer
  {

    private HashSet<string> BuiltInFilters = new HashSet<string>
    {

      "hint",
      "type",
      "cloze",
      "text"

    };

    private string ApplyQuestionFieldFilters(List<string> filters, string input)
    {

      if (filters.IsNull() || !filters.Any())
        return input;

      if (input.IsNullOrEmpty())
        return input;

      foreach (var filter in filters)
      {

        if (filter == "hint")
          input = HintFilter(input);

        else if (filter == "type")
          input = QuestionTypeFilter(input);

        else if (filter == "cloze")
          input = QuestionClozeFilter(input);

        else if (filter == "text")
          input = TextFilter(input);

      }

      return input;

    }

    private string ApplyAnswerFieldFilters(List<string> filters, string input)
    {

      if (filters.IsNull() || !filters.Any())
        return input;

      if (input.IsNullOrEmpty())
        return input;

      foreach (var filter in filters)
      {

        if (filter == "hint")
          input = HintFilter(input);

        else if (filter == "type")
          input = AnswerTypeFilter(input);

        else if (filter == "cloze")
          input = AnswerClozeFilter(input);

        else if (filter == "text")
          input = TextFilter(input);

      }

      return input;

    }
  }
}
