using Anotar.Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public partial class Renderer
  {

    public void AnswerClozeFilter(string fieldName)
    {

      if (!AnswerContent.TryGetValue(fieldName, out var renderContent))
      {
        LogTo.Error($"Failed to apply Create to field {fieldName} because the content dict does not contain a corresponding key");
        return;
      }

      string content = renderContent.Content;

      if (string.IsNullOrEmpty(content))
      {
        LogTo.Error("Failed to CreateClozeAnswer because fieldContent is null");
        return;
      }

      Match match = ClozeRegex.Match(content);
      var answerList = new List<string>();

      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze");
          continue;
        }

        int cardClozeNumber = Card.Ordinal + 1;

        // If the cloze number == cardOrdinal + 1,
        // add the answer to the answerList 
        if (clozeNumber == cardClozeNumber)
        {
          answerList.Add(match.Groups[2].Value);
        }
        match = match.NextMatch();
      }

      // Create the answerString
      string answerString = string.Empty;
      if (answerList != null && answerList.Count > 0)
      {

        // Create a list of answers
        for (int i = 0; i < answerList.Count; i++)
        {
          answerString += $"{i + 1}: {answerList[i]}";
        }

      }

      renderContent.Content = answerString;
      renderContent.AppliedFilters.Add("cloze");

    }

    public void AnswerTypeFilter(string fieldName)
    {

      if (!AnswerContent.TryGetValue(fieldName, out var renderContent))
      {
        LogTo.Error($"Failed to apply Create to field {fieldName} because the content dict does not contain a corresponding key");
        return;
      }

      renderContent.UnappliedFilters.Add("type");

    }
  }
}
