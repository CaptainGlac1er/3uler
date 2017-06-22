using Bot3ulerLogic;
using Bot3ulerLogic.Config.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3ulerBotServer
{
    /// <summary>
    /// Interaction logic for DiscordGuildsSidebar.xaml
    /// </summary>
    public partial class DiscordGuildsSidebar : UserControl
    {
        public DiscordGuildViewModel model;
        public DiscordGuildsSidebar()
        {
            model = new DiscordGuildViewModel();
            InitializeComponent();
            this.DataContext = model;
        }

        private void BtnAddChannel(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(((ChannelObject)(sender as Button).DataContext).Name);
        }
    }
    public class DiscordGuildViewModel : INotifyPropertyChanged,  IServerObserver<List<GuildObject>>
    {
        public string test = "test";
        public DiscordGuildViewModel()
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
        public void BotUpdate(List<GuildObject> update)
        {
            Debug.WriteLine("update sent");
            Guilds = update;

        }
    }
}
