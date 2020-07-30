using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SuperMemoAssistant.Plugins.CardSearcher.UI
{
  public class CardTemplateSelector : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {

      if (value is Card)
      {
        return (value as Card).Template.Name;
      }
      return "Error: value is not of type Card";

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

  public class DeckBaseNameSelector : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Card)
      {
        return (value as Card).Deck.Basename;
      }
      return "Error: value is not of type Card";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
