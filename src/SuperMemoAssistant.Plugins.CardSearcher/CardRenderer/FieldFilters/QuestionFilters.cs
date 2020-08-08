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
  public static class QuestionFilters
  {

    public static string QuestionClozeFilter(string content, Card card)
    {

      if (string.IsNullOrEmpty(content))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return string.Empty;
      }

      // Search for the cloze
      Match match = AnkiRegexes.ClozeRegex.Match(content);
      if (!match.Success || match.Groups.Count < 3)
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze regex didn't match");
        return string.Empty;
      }

      string question = string.Empty;

      int prevIndex = 0;
      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze.");
          continue;
        }

        int cardClozeNumber = card.Ordinal + 1;
        string clozeText = match.Groups[2].Value;
        int matchStart = match.Index;
        int matchEnd = match.Index + match.Length;

        // Get cloze hint if exists
        string clozeHint = null;
        if (match.Groups.Count >= 4)
        {
          clozeHint = match.Groups[3].Value;
        }

        if (clozeNumber == cardClozeNumber)
        {

          question += content.Substring(prevIndex, matchStart - prevIndex);
          question += "<span class=\"cloze\">[";
          // Add hint or ...
          question += string.IsNullOrEmpty(clozeHint)
            ? "..."
            : clozeHint;
          question += "]</span>";
          prevIndex = matchEnd;

        }
        else
        {

          question += content.Substring(prevIndex, matchStart - prevIndex);
          question += clozeText;
          prevIndex = matchEnd;

        }
        match = match.NextMatch();
      }

      // If questionstring not null, add the end part.
      if (!string.IsNullOrEmpty(question))
      {

        question += content.Substring(prevIndex);

      }

      return question;

    }

    // TODO:
    public static string QuestionTypeFilter(string input)
    {

      return input;

    }
  }
}
