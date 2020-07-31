using Forge.Forms.Annotations;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher
{
  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
      IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
      "Cancel",
      IsCancel = true)]
  [DialogAction("save",
      "Save",
      IsDefault = true,
      Validates = true)]
  public class CardSearcherCfg : CfgBase<CardSearcherCfg>, INotifyPropertyChangedEx
  {
    [Title("Card Searcher Plugin")]

    [Heading("By Jamesb | Experimental Learning")]

    [Heading("Features:")]
    [Text(@"- Search for similar cards")]

    [Heading("General Settings")]
    [Field(Name = "Activated?")]
    public bool Activated { get; set; } = true;

    [Field(Name = "Path to Anki collection?")]
    public string AnkiCollectionPath { get; set; } = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";

    [Field(Name = "Path to Anki media?")]
    public string AnkiMediaPath { get; set; }

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "Card Searcher Settings";
    }

    public event PropertyChangedEventHandler PropertyChanged;

  }
}
