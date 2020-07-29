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
      "cloze"

    };


    // Uses the QuestionContent dict
    private void ApplyQuestionFieldFilters(string fieldName, List<string> filters)
    {

      if (!QuestionContent.TryGetValue(fieldName, out var content))
      {
        LogTo.Error("Failed to find fieldName in the QuestionContent dictionary");
        return;
      }

      foreach (var filter in filters)
      {

        if (filter == "hint")
        {

        }
        else if (filter == "type")
        {

        }
        else if (filter == "cloze")
        {

        }

      }
    }

    private void ApplyAnswerFieldFilters(string fieldName, List<string> filters)
    {

      if (!AnswerContent.TryGetValue(fieldName, out var content))
      {
        LogTo.Error("Failed to find fieldName in the QuestionContent dictionary");
        return;
      }

      foreach (var filter in filters)
      {

        if (filter == "hint")
        {

        }
        else if (filter == "type")
        {

        }
        else if (filter == "cloze")
        {

        }
      }

    }
  }
}
