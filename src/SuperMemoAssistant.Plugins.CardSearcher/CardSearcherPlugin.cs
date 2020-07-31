#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   7/24/2020 7:01:17 PM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.CardSearcher
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Runtime.Remoting;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Anotar.Serilog;
  using Lunr;
  using SuperMemoAssistant.Extensions;
  using SuperMemoAssistant.Interop.SuperMemo.Core;
  using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
  using SuperMemoAssistant.Plugins.CardSearcher.Models;
  using SuperMemoAssistant.Plugins.CardSearcher.UI;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.HotKeys;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Services.UI.Configuration;
  using SuperMemoAssistant.Sys.Remoting;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class CardSearcherPlugin : SentrySMAPluginBase<CardSearcherPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public CardSearcherPlugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion

    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "CardSearcher";

    /// <inheritdoc />
    public override bool HasSettings => false;
    public CardSearcherCfg Config;
    public Dictionary<string, Card> Cards { get; set; }
    private CardWdw CurrentWdw { get; set; }

    #endregion

    #region Methods Impl

    private static readonly char[] PunctuationAndSymbols = new char[]
    {
      '.', '!', '?', ')', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '\\', '/', '<', '>', ',', ':', '[', ']'
    };

    public Index CardIndex { get; set; }


    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<CardSearcherCfg>() ?? new CardSearcherCfg();
    }

    /// <inheritdoc />
    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(HotKeyManager.Instance, Config);
    }

    /// <inheritdoc />
    protected override async void PluginInit()
    {

      LoadConfig();

      Cards = await GetAllCardsAsync().ConfigureAwait(false);
      await CreateCardIndexAsync().ConfigureAwait(false);

      Svc.SM.UI.ElementWdw.OnElementChanged += new ActionProxy<SMDisplayedElementChangedEventArgs>(OnElementChanged);

    }

    #endregion

    #region Methods

    private async Task<Dictionary<string, Card>> GetAllCardsAsync()
    {

      var ret = new Dictionary<string, Card>();
      var db = new DataAccess(Config.AnkiCollectionPath);
      var decks = await db.GetDecksAsync().ConfigureAwait(false);
      if (decks.IsNull() || !decks.Any())
        return ret;

      foreach (var deck in decks)
      {
        foreach (var card in deck.Value.Cards)
        {
          ret.Add(card.Id.ToString(), card);
        }
      }

      return ret;

    }

    private async Task CreateCardIndexAsync()
    {

      if (Cards.IsNull() || !Cards.Any())
        return;

      CardIndex = await Index.Build(async builder =>
      {

        builder
          .AddField("question")
          .AddField("answer");

        foreach (var card in Cards)
        {

          string question = card.Value.Question.InnerText();
          string answer = card.Value.Question.InnerText();
          string deck = card.Value.Deck.Basename;
          string id = card.Value.Id.ToString();

          await builder.Add(
          new Document {

            { "question",   question },
            { "answer",     answer },
            { "deck",       deck },
            { "id",         id }

          }).ConfigureAwait(false);
        }
      }).ConfigureAwait(false);

    }

    public async void OnElementChanged(SMDisplayedElementChangedEventArgs e)
    {
      try
      { 

        var element = e.NewElement;
        if (element.IsNull())
          return;

        if (element.Type != ElementType.Item)
          return;

        // Cancel search early on element changed event
        var cts = new RemoteCancellationTokenSource();
        Svc.SM.UI.ElementWdw.OnElementChanged += new ActionProxy<SMDisplayedElementChangedEventArgs>((args) => cts?.Cancel());

        List<string> searchTerms = GetSearchTerms();
        await SearchForCards(searchTerms, cts.Token);

      }
      catch (RemotingException) { }
    }

    private List<string> GetSearchTerms()
    {

      var ret = new List<string>();

      var allInnerText = ContentUtils.GetHtmlCtrlsInnerText();
      if (allInnerText.IsNull() || !allInnerText.Any())
        return ret;

      foreach (var kvpair in allInnerText)
      {

        var innerText = kvpair.Value;
        if (innerText.IsNullOrEmpty())
          continue;

        int idx = innerText.IndexOf("#SuperMemo Reference:");
        if (idx != -1)
          innerText = innerText.Substring(0, idx + 1);

        ret.AddRange(SplitIntoWords(innerText));

      }

      return ret;

    }

    private IEnumerable<string> SplitIntoWords(string text)
    {

      var ret = new List<string>();

      if (text.IsNullOrEmpty())
        return ret;

      return text
        ?.Split((char[])null)
        ?.Select(word => word.Trim(PunctuationAndSymbols))
        ?.Where(word => !word.IsNullOrEmpty())
        ?.Select(word => word.ToLowerInvariant())
        ?.Where(word => !Stopwords.English.Contains(word));

    }

    private void OpenCardWdw(List<Card> cards)
    {

      if (cards.IsNull() || !cards.Any())
        return;

      Application.Current.Dispatcher.Invoke(() =>
      {

        CurrentWdw = new CardWdw(cards);
        CurrentWdw.ShowAndActivate();

      });

    }

    private async Task SearchForCards(List<string> words, RemoteCancellationToken ct)
    {

      if (words.IsNull() || !words.Any())
        return;

      var results = new Dictionary<string, int>();

      try
      {

        foreach (var word in words)
        {

          if (word.IsNullOrEmpty() || ct.IsNull())
            return;

          if (CardIndex.IsNull())
            return;

          await foreach (Result res in CardIndex.Search(word, ct.Token()))
          {

            string id = res.DocumentReference;
            if (results.ContainsKey(id))
              results[id]++;
            else
              results[id] = 1;

          }
        }
      }
      catch (TaskCanceledException) { }

      if (results.IsNull() || !results.Any())
        return;

      var ordered = results
        .OrderByDescending(x => x.Value)
        .Take(Config.MaxSearchResults);

      var cards = new List<Card>();
      foreach (var pair in ordered)
      {
        cards.Add(Cards[pair.Key]);
      }

      if (CurrentWdw.IsNull() || CurrentWdw.IsClosed)
        OpenCardWdw(cards);
      else
        UpdateCardWdw(cards);

    }

    private void UpdateCardWdw(List<Card> cards)
    {

      Application.Current.Dispatcher.Invoke(() => 
      {

        CurrentWdw.ClearDataGrid();
        CurrentWdw.AddCards(cards);

      });

    }

    #endregion
  }
}
