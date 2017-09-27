using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _3ulerBotServer.ViewModel
{
    public class DiscordGuildSidebarViewModel : INotifyPropertyChanged, IServerObserver<List<GuildObject>>
    {
        public string test = "test";
        public DiscordGuildSidebarViewModel()
        {
            Guilds = new List<GuildObject>();
        }
        public List<GuildObject> guilds;
        public List<GuildObject> Guilds
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
                Debug.WriteLine(">" + propertyName);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public async Task BotUpdate(List<GuildObject> update)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine("update sent");
                Guilds = update;
            });
        }
    }
}
