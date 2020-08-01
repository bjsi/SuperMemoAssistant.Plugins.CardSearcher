using ServiceStack.DataAnnotations;
using SuperMemoAssistant.Plugins.CardSearcher.CardRenderer;
using SuperMemoAssistant.Plugins.CardSearcher.Models.Decks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models
{
  /// <summary>
  /// Enum describing the supported Card Types 
  /// </summary>
  public enum CardType
  {
    Cloze,
    Normal
  }

  /// <summary>
  /// Mapping the cards Table attributes
  /// </summary>
  [Alias("cards")]
  public class Card
  {

    /// <summary>
    /// time Epoch wehen the card was created
    /// </summary>
    [Alias("id")]
    public long Id { get; set; }

    /// <summary>
    /// note Identifier
    /// </summary>
    [Alias("nid")]
    public long NoteId { get; set; }

    /// <summary>
    /// Deck identifier
    /// </summary>
    [Alias("did")]
    public long DeckId { get; set; }


    /// <summary>
    /// orginal
    /// card templates   = 0 to num of templates -1 ???
    /// close deletions  = 0 to max cloze index -1  ???
    /// </summary>
    [Alias("ord")]
    public int Ordinal { get; set; }

    /// <summary>
    /// last modificaton time 
    /// </summary>
    [Alias("mod")]
    public long LastModificationTime { get; set; }

    /// <summary>
    /// update sequence number
    /// -1   = changes need to be synced
    /// else = changes that need to fetched from the server
    /// </summary>
    [Alias("usn")]
    public int UpdateSequenceNumber { get; set; }

    [Alias("type")]
    public int Type { get; set; }

    /// <summary>
    /// Describes the State of the card:
    ///     -3 = user buried  (In scheduler 2)  ///apprently there are schedulers???
    ///     -2 = sched buried (In scheduler 2)
    ///     -2 = buried       (In scheduler 1)
    ///     -1 = suspended
    ///      0 = new
    ///      1 = learning
    ///      2 = review
    ///      3 = in learning with at least a day after the previous review
    ///      4 = preview
    /// </summary>
    [Alias("queue")]
    public int Queue { get; set; }

    // TODO
    /// <summary>
    /// if the card is:
    /// New   : id is random int or the note id
    /// Due   : day relative to when the deck was created
    /// Learn : timestamp 
    /// </summary>
    [Alias("due")]
    public int due { get; set; }

    /// <summary>
    /// interval for Anki's algo
    /// -ve value : seconds
    /// +ve value : days
    /// </summary>
    [Alias("ivl")]
    public int Interval { get; set; }

    /// <summary>
    /// The ease factor of the card
    /// </summary>
    [Alias("factor")]
    public int Factor { get; set; }

    /// <summary>
    ///  number of reviews
    /// </summary>
    [Alias("reps")]
    public int Reps { get; set; }

    /// <summary>
    /// the number of times the card was not answered correctly
    /// </summary>
    [Alias("lapses")]
    public int Lapses { get; set; }

    /// <summary>
    ///  (the number of reps left today)*1000 + (the number of reps left till graduation) 
    /// </summary>
    [Alias("left")]
    public int Left { get; set; }

    /// <summary>
    /// original due: In filtered decks, it's the original due date that the card had before moving to filtered.
    /// If the card lapsed in scheduler1, then it's the value before the lapse. 
    ///  In any other case it's 0.
    /// </summary>
    [Alias("odue")]
    public int OriginalDue { get; set; }

    /// <summary>
    /// original deck  id
    /// </summary>
    [Alias("odid")]
    public int OriginalDeckId { get; set; }

    /// <summary>
    /// Red      1, 
    /// Orange   2, 
    /// Green    3, 
    /// Blue     4, 
    /// no flag: 0
    /// </summary>
    [Alias("flags")]
    public int Flags { get; set; }

    public Template Template
    {
      get
      {
        // TODO: Check this is correct
        return CardType == CardType.Cloze
          ? Note.NoteType.Templates.Where(t => t.Ordinal == 0).First()
          : Note.NoteType.Templates.Where(t => t.Ordinal == Ordinal).First();
      }
    }

    public CardType CardType
    {
      get
      {
        return Note.NoteType.Type == 1
          ? CardType.Cloze
          : CardType.Normal;
      }
    }

    // For UI
    public string QuestionFieldsBrowserPreview { 
      get
      {

        new Renderer(this).Render(TemplateType.Question, out var fieldDict);
        return UIEx.CreateBrowserPreviewFields(fieldDict);

      } 
    }

    // For UI
    public string AnswerFieldsBrowserPreview
    {
      get
      {

        new Renderer(this).Render(TemplateType.Answer, out var fieldDict);
        return UIEx.CreateBrowserPreviewFields(fieldDict);

      }
    }

    // For UI
    public string Question
    {
      get => new Renderer(this).Render(TemplateType.Question, out _);
    }

    // For UI
    public string Answer
    {
      get => new Renderer(this).Render(TemplateType.Answer, out _);
    }

    // Relationships
    [Reference]
    public Note Note { get; set; }

    public Deck Deck { get; set; }

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
}
