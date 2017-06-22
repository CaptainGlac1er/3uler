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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bot3ulerLogic;
using System.Diagnostics;

namespace _3ulerBotServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinkedList<string> previousCommands;
        ServerUpdater<string> consoleUpdater;
        Bot3uler bot;
        public MainWindow()
        {
            InitializeComponent();
            previousCommands = new LinkedList<string>();
            previousCommands.AddFirst("");
            consoleUpdater = new ServerUpdater<string>();
            consoleUpdater.AddObserver(consoleField.GetViewModel());
            bot = new Bot3uler(consoleUpdater);
            bot.ListenForGuildChange(GuildServers.model);
            bot.StartBot().GetAwaiter().OnCompleted(new Action(() => consoleUpdater.UpdateObservers("done")));
        }
        private LinkedListNode<string> head;
        private async void ConsoleKeyPress(object sender, KeyEventArgs e)
        {
            string input = consoleInput.Text;
            switch (e.Key)
            {
                case Key.Enter:
                    if (input != "")
                    {
                        consoleUpdater.UpdateObservers(string.Format("Console submitted {0}", input));
                        previousCommands.AddAfter(previousCommands.First, input);
                        head = previousCommands.First;
                        consoleInput.Text = "";
                    }
                    break;
                case Key.Home:
                    try
                    {
                        consoleUpdater.UpdateObservers("home pressed");
                        string update = await bot.GetStatus();
                        consoleUpdater.UpdateObservers(update);
                    }
                    catch (Exception er)
                    {
                        Debug.WriteLine(er.Message);
                    }
                    break;
                case Key.Up:
                    head = head.Next ?? head;
                    consoleInput.Text = (head != null) ? head.Value : "";
                    break;
                case Key.Down:
                    head = head.Previous ?? head;
                    consoleInput.Text = (head != null) ? head.Value : "";
                    break;
                case Key.Back:
                    if (input == "")
                        head = previousCommands.First;
                    break;

            }

        }

        private void BtnSubmitCommand(object sender, RoutedEventArgs e)
        {
            if (consoleInput.Text != "")
            {
                consoleUpdater.UpdateObservers(consoleInput.Text);
                consoleInput.Text = "";
            }
        }
    }
}
