using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models.Decks
{
  /// <summary>
  /// Represents an Anki Deck.
  /// </summary>
  public class Deck : INotifyPropertyChanged
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("collapsed")]
    public bool IsCollapsed { get; set; }

    [JsonProperty("conf")]
    public int ConfigId { get; set; }

    [JsonProperty("desc")]
    public string Description { get; set; }

    // TODO
    [JsonProperty("dyn")]
    public int dyn { get; set; }

    // TODO
    [JsonProperty("extendNew")]
    public int extendNew { get; set; }

    // TODO
    [JsonProperty("extendRev")]
    public int extendRev { get; set; }

    // TODO
    [JsonProperty("lrnToday")]
    public List<int> lrnToday { get; set; }

    [JsonProperty("mod")]
    public long LastModificationTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // TODO
    [JsonProperty("newToday")]
    public List<int> newToday { get; set; }

    // TODO
    [JsonProperty("revToday")]
    public List<int> revToday { get; set; }

    [JsonProperty("timeToday")]
    public List<int> timeToday { get; set; }

    // TODO
    [JsonProperty("usn")]
    public int usn { get; set; }

    /// <summary>
    /// Cards that are members of this deck. Does NOT include subdecks!
    /// </summary>
    public List<Card> Cards { get; set; } = new List<Card>();

    /// <summary>
    /// Deck config options
    /// </summary>
    public DeckConfig Config { get; set; }

    public int Level
    {
      get
      {
        return DeckNameEx.Level(Name);
      }
    }

    /// <summary>
    /// Holds whether the current Deck is root or not
    /// </summary>
    public bool IsRoot
    {
      get
      {
        return DeckNameEx.IsRoot(Name);
      }
    }

    public string Basename
    {
      get
      {
        return DeckNameEx.Basename(Name);
      }
    }

    public string Parentname
    {
      get
      {
        return DeckNameEx.Parentname(Name);
      }
    }

    public List<string> GetDeckPath()
    {
      return DeckNameEx.GetNamePath(Name);
    }

    // For UI
    /// <summary>
    /// Notifiy the UI for the changed Deck in Question.
    /// </summary>
    private bool _ToImport { get; set; } = false;
    public bool ToImport
    {
      get { return this._ToImport; }
      set
      {
        if (value != this._ToImport)
        {
          this._ToImport = value;
          NotifyPropertyChanged(nameof(ToImport));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }

  /// <summary>
  /// represents the DeckConfig
  /// </summary>
  public class DeckConfig
  {

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("autoplay")]
    public bool Autoplay { get; set; }

    // TODO
    [JsonProperty("dyn")]
    public bool dyn { get; set; }

    // TODO
    [JsonProperty("lapse")]
    public Dictionary<string, object> lapse { get; set; }

    // TODO
    [JsonProperty("maxTaken")]
    public int maxTaken { get; set; }

    [JsonProperty("mod")]
    public long LastModificationTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // TODO
    [JsonProperty("new")]
    public Dictionary<string, object> New { get; set; }

    [JsonProperty("replayq")]
    public bool ReplayQuestion { get; set; }

    // TODO
    [JsonProperty("rev")]
    public Dictionary<string, object> rev { get; set; }

    // TODO
    [JsonProperty("timer")]
    public int Timer { get; set; }

    // TODO
    [JsonProperty("usn")]
    public int usn { get; set; }
  }
}
