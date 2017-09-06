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
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : UserControl
    {
        ConsoleViewModel ViewModel;
        public Console()
        {
            InitializeComponent();
            ViewModel = new ConsoleViewModel();
            this.DataContext = ViewModel;
        }
        public ConsoleViewModel GetViewModel()
        {
            return ViewModel;
        }

        private void console_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleScroll.ScrollToEnd();
        }
    }
}
