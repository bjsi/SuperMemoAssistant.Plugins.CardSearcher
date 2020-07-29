using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher
{
  public static class ContentUtils
  {

    public static Dictionary<int, string> GetHtmlCtrlsInnerText()
    {

      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      if (ctrlGroup.IsNull() || ctrlGroup.IsDisposed)
        return null;

      var ret = new Dictionary<int, string>();

      for (int i = 0; i < ctrlGroup.Count; i++)
      {

        var htmlCtrl = ctrlGroup[i].AsHtml();
        var htmlDoc = htmlCtrl?.GetDocument();
        if (htmlDoc.IsNull())
          continue;

        ret.Add(i, htmlDoc.body?.innerText ?? string.Empty);

      }

      return ret;

    }

  }
}
