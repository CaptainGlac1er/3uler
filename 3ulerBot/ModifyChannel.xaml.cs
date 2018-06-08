using System;
using System.Collections.Generic;
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
using Bot3ulerLogic;
using Bot3ulerLogic.Config.Objects;
using _3ulerBotServer.ViewModel;
using _3ulerBotShared.Models;

namespace _3ulerBotServer
{
    /// <summary>
    /// Interaction logic for ModifyChannel.xaml
    /// </summary>
    public partial class ModifyChannel : Window
    {
        public ChannelViewModel model;
        public ModifyChannel(Channel channelBeingModified)
        {
            model = new ChannelViewModel
            {
                ChannelBeingModified = channelBeingModified
            };
            InitializeComponent();
            this.DataContext = model;
        }

        private void CloseChannelDialog_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnRemoveCommand(object sender, RoutedEventArgs e)
        {
            var commandBeingRemoved = ((sender as Button).DataContext as Command);
            if (commandBeingRemoved != null)
            {
                model.ChannelBeingModified.RemoveCommand(commandBeingRemoved.CommandId);
            }
        }

        private void BtnAddCommand_OnClick(object sender, RoutedEventArgs e)
        {
            var commandBeingAdded = (sender as Button)?.DataContext as Command;
            if (commandBeingAdded != null)
            {
                model.ChannelBeingModified.AddCommand(commandBeingAdded.CommandId);
            }
        }
    }
}
