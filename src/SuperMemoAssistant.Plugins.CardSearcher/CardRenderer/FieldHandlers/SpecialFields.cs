using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.CardRenderer.FieldHandlers
{

  public static class SpecialFields
  {

    public static string TagFieldHandler(Card card:)
    {
      return card.Note.Tags;
    }

    public static string TypeFieldHandler(Card card)
    {
      return card.Note.NoteType.Name;
    }

    public static string SubdeckFieldHandler(Card card)
    {
      return card.Note.NoteType.Name;
    }

    public static string DeckFieldHandler(Card card)
    {
      return card.Note.NoteType.Name;
    }

    public static string CardFieldHandler(Card card)
    {
      return card.Note.NoteType.Name;
    }

    public static string FrontSideFieldHandler(Card card)
    {
      return card.Note.NoteType.Name;
    }

  }
}
