using SuperMemoAssistant.Plugins.CardSearcher.CardRenderer;
using SuperMemoAssistant.Plugins.CardSearcher.Models;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public bool IsClosed { get; set; } = false;

    public CardWdw(List<Card> cards)
    {

      InitializeComponent();

      Closed += (sender, args) => IsClosed = true;

      AddCards(cards);

    }

    public void ClearDataGrid()
    {

      var cards = (CardCollection)this.Resources["cards"];
      cards?.Clear();

    }

    public void AddCards(List<Card> cards)
    {

      CardCollection _cards = (CardCollection)this.Resources["cards"];
      foreach (var card in cards)
      {
        _cards.Add(card);
      }

    }

    private void ImportBtn_Click(object sender, RoutedEventArgs e)
    {

      ICollectionView cards = CollectionViewSource.GetDefaultView(DG1.ItemsSource);

      Card card = cards
        .Cast<Card>()
        .Where(e => e.ToImport)
        .FirstOrDefault();

      if (card.IsNull())
      {
        MessageBox.Show("Failed to import card because card was null");
        return;
      }

      var builder = new AnkiCardBuilder(card);

      Svc.SM.Registry.Element.Add(
        out _,
        Interop.SuperMemo.Elements.Models.ElemCreationFlags.ForceCreate,
        builder.CreateElementBuilder()
      );

    }
  }
}
