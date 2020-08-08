using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class UIEx
  {

    public static string CreateBrowserPreviewFields(Dictionary<string, string> fieldDict)
    {

      if (fieldDict.IsNull() || !fieldDict.Any())
        return string.Empty;

      return String.Join("\n", fieldDict
        .Where(x => !x.Value.IsNullOrEmpty())
        .Select(x => x.Key + ": " + x.Value));

    }

  }
}
