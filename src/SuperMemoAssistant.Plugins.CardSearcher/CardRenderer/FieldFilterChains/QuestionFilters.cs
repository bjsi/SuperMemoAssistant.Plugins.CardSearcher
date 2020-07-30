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

    public void QuestionClozeFilter(string fieldName)
    {

      string question = string.Empty;

      if (!QuestionContent.TryGetValue(fieldName, out var renderedContent))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return;
      }

      var content = renderedContent.Content;

      if (string.IsNullOrEmpty(content))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return;
      }

      // Search for the cloze
      Match match = ClozeRegex.Match(content);
      if (!match.Success || match.Groups.Count < 3)
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze regex didn't match");
        return;
      }

      int prevIndex = 0;
      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze.");
          continue;
        }

        int cardClozeNumber = Card.Ordinal + 1;
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

      renderedContent.Content = question;
      renderedContent.AppliedFilters.Add("cloze");

    }

    public void QuestionTypeFilter(string fieldName)
    {

      if (!QuestionContent.TryGetValue(fieldName, out var renderContent))
      {
        LogTo.Error($"Failed to apply Create to field {fieldName} because the content dict does not contain a corresponding key");
        return;
      }

      renderContent.UnappliedFilters.Add("type");

    }
  }
}
