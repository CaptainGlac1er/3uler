using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bot3ulerLogic.Config.Objects;

namespace _3ulerBotServer.ViewModel
{
    public class DiscordGuildSidebarViewModel : INotifyPropertyChanged, IServerObserver<List<Guild>>
    {
        public string test = "test";
        public DiscordGuildSidebarViewModel()
        {
            Guilds = new List<Guild>();
        }
        public List<Guild> guilds;
        public List<Guild> Guilds
        {
            get
            {
                return this.guilds;
            }
            set
            {
                if (value != this.guilds)
                {
                    this.guilds = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public async Task BotUpdate(List<Guild> update)
        {
            await Task.Run(() =>
            {
                Guilds = update;
            });
        }
    }
}
