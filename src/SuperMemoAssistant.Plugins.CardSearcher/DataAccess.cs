using Anotar.Serilog;
using ServiceStack.OrmLite;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
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
