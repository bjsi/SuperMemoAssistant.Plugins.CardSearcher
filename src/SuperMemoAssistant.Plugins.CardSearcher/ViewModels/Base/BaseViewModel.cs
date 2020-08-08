using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.CardSearcher.ViewModels.Base
{

  public abstract class BaseViewModel : INotifyPropertyChangedEx
  {

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

  }

}
