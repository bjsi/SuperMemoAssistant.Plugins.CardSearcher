using SuperMemoAssistant.Plugins.CardSearcher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperMemoAssistant.Plugins.CardSearcher.UI
{

  public static class BrowserBehavior
  {
    public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowserBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

    [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
    public static string GetHtml(WebBrowser d)
    {
      return (string)d.GetValue(HtmlProperty);
    }

    public static void SetHtml(WebBrowser d, string value)
    {
      d.SetValue(HtmlProperty, value);
    }

    static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      WebBrowser webBrowser = dependencyObject as WebBrowser;
      if (webBrowser != null)
        webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
    }
  }

  public class CardCollection : ObservableCollection<Card>
  {
  }

  /// <summary>
  /// Interaction logic for CardWdw.xaml
  /// </summary>
  public partial class CardWdw : Window
  {
    public CardWdw(List<Card> cards)
    {

      if (cards.IsNull() || !cards.Any())
        return;

      InitializeComponent();

      CardCollection _cards = (CardCollection)this.Resources["cards"];
      foreach (var card in cards)
      {
        _cards.Add(card);
      }
    }
  }
}
