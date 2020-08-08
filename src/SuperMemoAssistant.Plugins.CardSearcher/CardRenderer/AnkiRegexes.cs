using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer
{
  public static class AnkiRegexes
  {

    // (1: cloze number), (2: text), (3: hint)
    public static readonly Regex ClozeRegex = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");

    // The Regex pattern for audio and video tags
    public static readonly string AudioVideoPattern = @"(?xs)\[sound: (.*?)\]";

    // Video is also in the [sound:...] tag
    public static readonly Regex AudioVideoRegex = new Regex(@"(?xs)\[sound: (.*?)\]");

  }
}
