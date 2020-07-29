using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models
{
  /// <summary>
  /// Represents an Anki Collection
  /// </summary>
  [Alias("col")]
  public class Collection
  {

    /// <summary>
    ///  arbitrary identifier to represent the only single row about the collection in anki
    /// </summary>
    [Alias("id")]
    public long Id { get; set; }

    /// <summary>
    /// creation Date for the Deck
    /// </summary>
    [Alias("crt")]
    public long CreatedAt { get; set; }

    /// <summary>
    /// The last time the card was modified
    /// </summary>
    [Alias("mod")]
    public long LastModificationTime { get; set; }

    // TODO
    /// <summary>
    ///represents the last time the deck was modified
    /// </summary>
    [Alias("scm")]
    public long scm { get; set; }

    /// <summary>
    /// Some version number --Not sure what vrson does it represent yet
    /// </summary>
    [Alias("ver")]
    public int VersionNumber { get; set; }

    /// <summary>
    /// Dirty ie unused, usually set to 0
    /// </summary>
    [Alias("dty")]
    public int dty { get; set; }

    // TODO
    /// <summary>
    /// usn  stands for the update sequence number ie used for figuring out the difference when syncing
    /// if -1 any changes to the deck need to be synced or rathter pushed to the server
    /// </summary>
    [Alias("usn")]
    public int UpdateSequenceNumber { get; set; }

    /// <summary>
    /// last time the deck was synced at
    /// </summary>
    [Alias("ls")]
    public long LastSync { get; set; }

    /// <summary>
    /// configuration options for hte deck
    /// </summary>
    [Alias("conf")]
    public string Config { get; set; }

    /// <summary>
    /// note types in the deck
    /// inlding formatting and such
    /// </summary>
    [Alias("models")]
    public string NoteTypes { get; set; }

    /// <summary>
    /// deck options
    /// </summary>
    [Alias("dconf")]
    public string DeckConfigs { get; set; }


    /// <summary>
    /// Array containing deck options
    /// </summary>
    [Alias("decks")]
    public string Decks { get; set; }

    /// <summary>
    /// Cached tags for the deck
    /// Note: for some reason they are set to -1
    /// </summary>
    [Alias("tags")]
    public string Tags { get; set; }
  }
}
