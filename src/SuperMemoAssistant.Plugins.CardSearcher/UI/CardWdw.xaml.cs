using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
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
    public Card SelectedCard {get;set;}

    public CardWdw(List<Card> cards)
    {

      InitializeComponent();

      Closed += (sender, args) => IsClosed = true;

      AddCards(cards);

      DG1.SelectionChanged += DG1_SelectionChanged;

      DataContext = this;

    }

    private void DG1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

      if (SelectedCard.IsNull())
      {
        AnswerPreviewer.NavigateToString("&nbsp;");
        QuestionPreviewer.NavigateToString("&nbsp;");
      }
      else
      {
        AnswerPreviewer.NavigateToString(SelectedCard.Answer);
        QuestionPreviewer.NavigateToString(SelectedCard.Question);
      }

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

      TemplateRenderOptions opts = new TemplateRenderOptions();
      opts.Refs
        .WithAuthor(AuthorTextbox.Text)
        .WithTitle(TitleTextbox.Text)
        .WithAuthor(SourceTextbox.Text);

      if (ImageExtractionCheckbox.IsChecked == true)
        opts.AddImageComponents = true;
      else
        opts.AddImageComponents = false;

      ElementBuilder builder = null;

      if (IgnoreDuplicateFieldsCheckbox.IsChecked == true)
      {
        var question = new Renderer(card).Render(TemplateType.Question, out var fieldDict);
        var answer = new Renderer(card).RenderAnswerIgnoreDuplicates(fieldDict, out _);
        builder = new AnkiCardBuilder(card, question, answer).CreateElementBuilder();
      }
      else
      {
        builder = new AnkiCardBuilder(card).CreateElementBuilder();
      }

      IElement parent = null;

      double priority = PrioritySlider.Value;
      if (priority < 0 || priority > 100)
      {
        priority = 30;
      }

      if (ImportChildRadio.IsChecked == true)
        parent = Svc.SM.UI.ElementWdw.CurrentElement;

      Svc.SM.Registry.Element.Add(
        out _,
        Interop.SuperMemo.Elements.Models.ElemCreationFlags.ForceCreate,
        builder
          .WithParent(parent)
          .WithPriority(priority)
      );

    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void PrioritySlider_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.PageDown)
      {
        PrioritySlider.Value = PrioritySlider.Value <= 95
          ? PrioritySlider.Value + 5
          : 100;

        e.Handled = true;
      }
      else if (e.Key == Key.PageUp)
      {

        PrioritySlider.Value = PrioritySlider.Value >= 5
          ? PrioritySlider.Value - 5
          : 0;

        e.Handled = true;
      }

    }

    private TextBox FindActiveReferencesTextbox()
    {

      if (AuthorTextbox.IsFocused)
        return AuthorTextbox;

      else if (TitleTextbox.IsFocused)
        return TitleTextbox;

      else if (LinkTextbox.IsFocused)
        return LinkTextbox;

      else if (SourceTextbox.IsFocused)
        return TitleTextbox;

      else if (EmailTextbox.IsFocused)
        return TitleTextbox;

      return null;

    }

    private void SubdeckNamePlaceholderBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

      var textbox = FindActiveReferencesTextbox();
      if (textbox.IsNull())
        return;

      if (textbox.SelectedText == string.Empty)
        textbox.SelectedText = "${SubdeckName}";

      e.Handled = true;

    }

    private void DeckNamePlaceholderBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

      var textbox = FindActiveReferencesTextbox();
      if (textbox.IsNull())
        return;

      if (textbox.SelectedText == string.Empty)
        textbox.SelectedText = "${DeckName}";

      e.Handled = true;

    }

    private void CardTypePlaceholderBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

      var textbox = FindActiveReferencesTextbox();
      if (textbox.IsNull())
        return;

      if (textbox.SelectedText == string.Empty)
        textbox.SelectedText = "${CardType}";

      e.Handled = true;

    }

    private void NoteTypePlaceholderBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

      var textbox = FindActiveReferencesTextbox();
      if (textbox.IsNull())
        return;

      if (textbox.SelectedText == string.Empty)
        textbox.SelectedText = "${NoteType}";

      e.Handled = true;

    }
  }
}
