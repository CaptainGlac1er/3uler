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
using _3ulerBotServer.ViewModel;

namespace _3ulerBotServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinkedList<string> previousCommands;
        MainWindowViewModel ViewModel;
        
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            previousCommands = new LinkedList<string>();
            previousCommands.AddFirst("");
            ViewModel.AddConsole(consoleField.GetViewModel());
            ViewModel.AddBot();
            ViewModel.AddGuildList(GuildServers.model);
            this.Loaded += MainWindow_Loaded;//.GetAwaiter().OnCompleted(new Action(() => consoleUpdater.UpdateObservers("done")));
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.GetBot().StartBot();
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
                        await ViewModel.ConsoleUpdater.UpdateObservers(string.Format("Console submitted {0}", input));
                        previousCommands.AddAfter(previousCommands.First, input);
                        head = previousCommands.First;
                        consoleInput.Text = "";
                    }
                    break;
                case Key.Home:
                    try
                    {
                        await ViewModel.ConsoleUpdater.UpdateObservers("home pressed");
                        string update = await ViewModel.GetBot().GetStatus();
                        await ViewModel.ConsoleUpdater.UpdateObservers(update);
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

        private async void BtnSubmitCommand(object sender, RoutedEventArgs e)
        {
            if (consoleInput.Text != "")
            {
                await ViewModel.ConsoleUpdater.UpdateObservers(consoleInput.Text);
                consoleInput.Text = "";
            }
        }
    }
}
