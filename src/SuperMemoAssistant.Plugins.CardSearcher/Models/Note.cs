using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models
{
  [Alias("notes")]
  public class Note
  {
    /// <summary>
    /// when the note was created (milliseconds)
    /// </summary>
    [Alias("id")]
    public long Id { get; set; }

    /// <summary>
    /// global unique Id used for syncing
    /// </summary>
    [Alias("guid")]
    public string Guid { get; set; }

    /// <summary>
    /// model ID or the type of the note
    /// </summary>
    [Alias("mid")]
    public long NoteTypeId { get; set; }

    /// <summary>
    /// last time a certain note was modeified (seconds)
    /// </summary>
    [Alias("mod")]
    public long LastModificationTime { get; set; }

    /// <summary>
    /// update sequence number
    /// </summary>
    [Alias("usn")]
    public int UpdateSequenceNumber { get; set; }

    /// <summary>
    /// tags associated with that card
    /// </summary>
    [Alias("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// basically the card data
    /// </summary>
    [Alias("flds")]
    public string FieldString { get; set; }

    /// <summary>
    /// used for quick sorting and checking for duplicate notes 
    /// </summary>
    /*  defined as an integer in the Anki db. Contains text as well of the first field
     * [Alias("sld")]
     * public string SortField { get; set; }
     */

    /// <summary>
    /// first 8 digits of the sha1-hash of the first field
    /// </summary>
    [Alias("csum")]
    public long Checksum { get; set; }

    [Alias("flags")]
    public int Flags { get; set; }

    public Dictionary<string, string> Fields
    {
      get
      {
        var fieldContentMap = new Dictionary<string, string>();
        // fields are seprated by 0x1f 
        var values = FieldString.Split('\x1f');
        var keys = NoteType.Fields;
        keys.ForEach(k => fieldContentMap.Add(k.Name, values[k.Ordinal]));
        return fieldContentMap;
      }
    }

    /// <summary>
    /// Note: I think this has to be based on the notetype id
    /// </summary>
    public NoteType NoteType { get; set; }
  }
}
