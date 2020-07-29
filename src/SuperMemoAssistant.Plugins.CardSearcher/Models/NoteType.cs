using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.Models
{

  public class NoteType
  {
    [JsonProperty("sortf")]
    public string SortField { get; set; }

    [JsonProperty("did")]
    public long DeckId { get; set; }

    [JsonProperty("latexPre")]
    public string LatexPre { get; set; }

    [JsonProperty("latexPost")]
    public string LatexPost { get; set; }

    [JsonProperty("mod")]
    public long LastModificationTime { get; set; }

    [JsonProperty("usn")]
    public int UpdateSequenceNumber { get; set; }

    [JsonProperty("vers")]
    public object[] VersionNumber { get; set; }

    [JsonProperty("css")]
    public string CSS { get; set; }

    // TODO
    [JsonProperty("req")]
    public object[] req { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("flds")]
    public List<Field> Fields { get; set; }

    [JsonProperty("tmpls")]
    public List<Template> Templates { get; set; }

  }

  // TODO: Needs a reference to its NoteType?
  public class Template
  {

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("bafmt")]
    public string BrowserAnswerFormat { get; set; }

    [JsonProperty("bqfmt")]
    public string BrowserQuestionFormat { get; set; }

    [JsonProperty("ord")]
    public int Ordinal { get; set; }

    [JsonProperty("qfmt")]
    public string QuestionFormat { get; set; }

    [JsonProperty("afmt")]
    public string AnswerFormat { get; set; }
  }

  public class Field
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    // TODO
    [JsonProperty("media")]
    public object[] media { get; set; }

    [JsonProperty("ord")]
    public int Ordinal { get; set; }

    [JsonProperty("rtl")]
    public bool RightToLeftScript { get; set; }

    [JsonProperty("font")]
    public string FontStyle { get; set; }

    [JsonProperty("size")]
    public int FontSize { get; set; }

    // TODO
    /// <summary>
    /// contain the value of the last thing that was added
    /// </summary>
    [JsonProperty("sticky")]
    public bool sticky { get; set; }

  }
}
