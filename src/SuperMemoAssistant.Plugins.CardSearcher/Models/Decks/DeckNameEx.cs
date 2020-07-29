using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models.Decks
{
  /// <summary>
  /// Some extension methods for dealing with deck names
  /// Useful for searching deck tree.
  /// </summary>
  public static class DeckNameEx
  {

    /// <summary>
    /// Returns whether a deck is a root deck, based on whether it contains anki's deck-subdeck separator
    /// </summary>
    /// <param name="name"></param>
    public static bool IsRoot(string name)
    {
      return string.IsNullOrEmpty(name)
        ? false
        : !name.Contains("::");
    }

    /// <summary>
    /// Converts the name of a deck, eg: parent::child::grandchild to a list [parent, parent::child, parent::child::grandchild]
    /// Used for creating an iterable 'path' through a deck tree dictionary.
    /// </summary>
    /// <param name="name"></param>
    public static List<string> GetNamePath(string name)
    {
      if (string.IsNullOrEmpty(name))
        return null;

      var path = new List<string>();
      List<string> nameSplit = name.Split(new string[] { "::" }, StringSplitOptions.None).ToList();
      for (int i = 0; i <= Level(name); i++)
      {
        path.Add(string.Join("::", nameSplit.Take(i + 1)));
      }
      return path;
    }

    /// <summary>
    /// Get the name of the parent of the deck.
    /// eg. parent::child would return parent.
    /// </summary>
    /// <param name="name"></param>
    public static string Parentname(string name)
    {
      List<string> parentNameSplit = name
        ?.Split(new string[] { "::" }, StringSplitOptions.None)
        ?.Take(Level(name))
        ?.ToList();

      return parentNameSplit != null && parentNameSplit.Count != 0
        ? string.Join("::", parentNameSplit)
        : null;
    }

    /// <summary>
    /// Return the last section in the name.
    /// eg. parent::child::grandchild returns grandchild.
    /// </summary>
    /// <param name="name"></param>
    public static string Basename(string name)
    {
      return string.IsNullOrEmpty(name)
        ? null
        : name?.Split(new string[] { "::" }, StringSplitOptions.None)?.Last();
    }

    /// <summary>
    /// Gets the level of the deck in the hierarchy. Zero-indexed: Root decks have level 0.
    /// Direct child of root has level 1.
    /// Returns -1 on invalid input.
    /// </summary>
    public static int Level(string name)
    {
      if (string.IsNullOrEmpty(name))
        return -1;
      return name?.Split(new string[] { "::" }, StringSplitOptions.None)?.Length - 1 ?? -1;
    }
  }
}
