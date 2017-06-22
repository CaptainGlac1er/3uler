using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _3ulerBotServer.ViewModel
{
    public class ConsoleViewModel : INotifyPropertyChanged, IServerObserver<string>
    {
        public ConsoleViewModel()
        {
            console = "";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private string console;
        public string Console
        {
            get
            {
                return console;
            }
            set
            {
                if(value != this.console)
                {
                    this.console = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void BotUpdate(string update)
        {
            WriteLine(update);
        }
        private void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
        private void WriteLine(string line)
        {
            Console = $"{Console}{line}\n";
        }
    }
}
