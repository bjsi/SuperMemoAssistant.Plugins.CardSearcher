using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public class FilterNode
  {

    private Dictionary<string, string> input;
    private string key;

    private Func<Dictionary<string, string>, string, string> Current;
    private Func<Dictionary<string, string>, string, string> Next;

    public FilterNode(Func<Dictionary<string, string>, string, string> current, Dictionary<string, string> input, string key)
    {

      this.Current = current;

    }

    public FilterNode(Func<Dictionary<string, string>, string, string> current,
                      Func<Dictionary<string, string>, string, string> next,
                      Dictionary<string, string> input,
                      string key)
    {

      this.Current = current;
      this.Next = next;

    }

    public void Begin()
    {

    }

  }
}
