using _3ulerBotServer.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
            System.Console.WriteLine("sup nerd");
            ViewModel = new MainWindowViewModel();
            previousCommands = new LinkedList<string>();
            previousCommands.AddFirst("");
            ViewModel.AddConsole(consoleField.GetViewModel());
            ViewModel.AddGuildList(GuildServers.model);
            this.Loaded += MainWindow_Loaded;//.GetAwaiter().OnCompleted(new Action(() => consoleUpdater.UpdateObservers("done")));
            System.Console.WriteLine("finished");
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
                        await consoleField.GetViewModel().BotUpdate(string.Format("Console submitted {0}", input));
                        previousCommands.AddAfter(previousCommands.First, input);
                        head = previousCommands.First;
                        consoleInput.Text = "";
                    }
                    break;
                case Key.Home:
                    try
                    {
                        await consoleField.GetViewModel().BotUpdate("home pressed");
                        string update = await ViewModel.GetBot().GetStatus();
                        await consoleField.GetViewModel().BotUpdate(update);
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
                await consoleField.GetViewModel().BotUpdate(consoleInput.Text);
                consoleInput.Text = "";
            }
        }
    }
}
