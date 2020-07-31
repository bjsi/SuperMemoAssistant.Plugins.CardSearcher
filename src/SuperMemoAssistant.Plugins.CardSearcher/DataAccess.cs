using Anotar.Serilog;
using ServiceStack.OrmLite;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using SuperMemoAssistant.Plugins.CardSearcher.Models.Decks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher
{
  public class DataAccess
  {

    public OrmLiteConnectionFactory dbFactory;
    private string database { get; set; }

    public DataAccess(string database)
    {
      this.database = database;
      this.dbFactory = new OrmLiteConnectionFactory(database, SqliteDialect.Provider);
    }

    //
    // JSON Helpers
    public static Dictionary<long, NoteType> GetNoteTypesObject(string NoteTypes)
    {
      return NoteTypes.Deserialize<Dictionary<long, NoteType>>();
    }

    public static NoteType GetNoteType(Dictionary<long, NoteType> NoteTypes, long id)
    {

      NoteType noteType = null;
      NoteTypes?.TryGetValue(id, out noteType);
      return noteType;

    }

    public static DeckConfig GetDeckConfig(string DeckConfigs, long id)
    {

      var deckConfigs = DeckConfigs.Deserialize<Dictionary<long, DeckConfig>>();
      DeckConfig config = null;
      deckConfigs?.TryGetValue(id, out config); //If no object is found maintain it's current state as null else set config it to the new object
      return config;

    }

    public static DeckConfig GetDeckConfig(Dictionary<long, DeckConfig> DeckConfigs, long id)
    {
      DeckConfig config = null;
      DeckConfigs?.TryGetValue(id, out config);
      return config;
    }

    public static Dictionary<long, DeckConfig> GetDeckConfigsObject(string DeckConfigs)
    {
      return DeckConfigs.Deserialize<Dictionary<long, DeckConfig>>();
    }

    public static Dictionary<long, Deck> GetDecksObject(string Decks)
    {
      return Decks.Deserialize<Dictionary<long, Deck>>();
    }

    //
    // Decks
    public async Task<Dictionary<long, Deck>> GetDecksAsync(Func<KeyValuePair<long, Deck>, bool> filter = null)
    {

      if (!File.Exists(database))
      {
        LogTo.Warning("Attempted to GetDecksAsync but DBPath does not exist");
        return null;
      }

      Dictionary<long, Deck> decks = new Dictionary<long, Deck>();

      try
      {
        using (var db = dbFactory.Open())
        {

          List<Collection> cols = await db.SelectAsync<Collection>();

          Collection col = cols?.FirstOrDefault();
          if (col == null)
          {
            LogTo.Debug("Failed to GetDecksAsync because collection was null");
            return decks;
          }

          var deckConfigs = GetDeckConfigsObject(col.DeckConfigs);
          var results = GetDecksObject(col.Decks);

          if (filter != null)
            results = results.Where(x => filter(x)).ToDictionary(x => x.Key, x => x.Value);

          if (results != null && results.Count > 0)
          {
            foreach (KeyValuePair<long, Deck> kvPair in results)
            {

              var deck = kvPair.Value;
              var config = GetDeckConfig(deckConfigs, deck.Id);
              deck.Config = config;
              var cards = await GetCardsAsync(x => x.DeckId == deck.Id);
              cards?.ForEach(c => c.Deck = deck);
              deck.Cards = cards;

            }
            decks = results;
          }
        }
      }
      catch (Exception e)
      {
        LogTo.Error($"Failed to GetDecksAsync with exception {e}");
      }
      return decks;
    }

    //
    // Notes
    public async Task<List<Note>> GetNotesAsync(Expression<Func<Note, bool>> predicate = null)
    {

      if (!File.Exists(database))
      {
        LogTo.Warning("Failed to GetNotesAsync because database does not exist");
        return null;
      }

      List<Note> notes = new List<Note>();

      try
      {
        using (var db = dbFactory.Open())
        {

          var noteTypes = await GetNoteTypesAsync().ConfigureAwait(false);

          // Apply filter if desired
          if (predicate != null)
            notes = await db.SelectAsync<Note>(predicate).ConfigureAwait(false);
          else
            notes = await db.SelectAsync<Note>(x => true).ConfigureAwait(false);

          if (notes != null && notes.Count > 0)
          {

            foreach (var note in notes)
            {
              note.NoteType = GetNoteType(noteTypes, note.NoteTypeId);
            }

          }
        }
      }
      catch (Exception e)
      {
        LogTo.Error($"Failed to GetNotesAsync with exception {e}");
      }

      return notes;
    }

    //
    // Note Types
    public async Task<Dictionary<long, NoteType>> GetNoteTypesAsync()
    {
      if (!File.Exists(database))
      {
        LogTo.Warning($"Failed to GetNoteTypesAsync because database {database} does not exist");
        return null;
      }

      var noteTypes = new Dictionary<long, NoteType>();

      try
      {
        using (var db = dbFactory.Open())
        {

          List<Collection> cols = await db.SelectAsync<Collection>().ConfigureAwait(false);
          Collection col = cols?.FirstOrDefault();
          noteTypes = GetNoteTypesObject(col?.NoteTypes);

        }

      }
      catch (Exception e)
      {
        LogTo.Error($"Failed to GetCardsAsync with exception {e}");
      }
      return noteTypes;
    }

    //
    // Cards
    public async Task<List<Card>> GetCardsAsync(Expression<Func<Card, bool>> predicate = null)
    {

      if (!File.Exists(database))
      {
        LogTo.Warning("Failed to GetCardsAsync because database does not exist");
        return null;
      }

      List<Card> cards = new List<Card>();

      try
      {
        using (var db = dbFactory.Open())
        {

          // Add the optional filter if passed or return all
          if (predicate != null)
            cards = await db.LoadSelectAsync<Card>(predicate).ConfigureAwait(false);
          else
            cards = await db.LoadSelectAsync<Card>(x => true).ConfigureAwait(false);

          // Doesn't come with NoteTypes so need to add them on
          var noteTypes = await GetNoteTypesAsync().ConfigureAwait(false);
          foreach (var card in cards)
          {
            NoteType noteType = null;
            noteTypes.TryGetValue(card.Note.NoteTypeId, out noteType);
            card.Note.NoteType = noteType;
          }
        }
      }
      catch (Exception e)
      {
        LogTo.Error($"Failed to GetCardsAsync with exception {e}");
      }
      return cards;
    }
  }
}
