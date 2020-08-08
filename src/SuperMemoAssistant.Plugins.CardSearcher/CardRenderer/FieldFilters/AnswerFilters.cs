using Anotar.Serilog;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class AnswerFilters
  {

    public static string AnswerClozeFilter(string content, Card card)
    {

      if (string.IsNullOrEmpty(content))
      {
        LogTo.Error("Failed to CreateClozeAnswer because fieldContent is null or empty");
        return string.Empty;
      }

      Match match = AnkiRegexes.ClozeRegex.Match(content);
      var answerList = new List<string>();

      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze");
          continue;
        }

        int cardClozeNumber = card.Ordinal + 1;

        // If the cloze number == cardOrdinal + 1,
        // add the answer to the answerList 
        if (clozeNumber == cardClozeNumber)
        {
          answerList.Add(match.Groups[2].Value);
        }
        match = match.NextMatch();
      }

      // Create the answerString
      string answer = string.Empty;
      if (answerList != null && answerList.Count > 0)
      {

        // Create a list of answers
        for (int i = 0; i < answerList.Count; i++)
        {
          answer += $"{i + 1}: {answerList[i]}";
        }

      }

      return answer;

    }

    // TODO:
    public static string AnswerTypeFilter(string input)
    {
      return input;
    }
  }
}
