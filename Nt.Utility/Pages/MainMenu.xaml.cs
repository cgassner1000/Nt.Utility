using Nt.Utility.Pages.Novatouch;
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

namespace Nt.Utility
{
    /// <summary>
    /// Interaktionslogik für MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }


        private void DatabaseTools_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DBConnection());
        }

        private void SoftwareInstall_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Fiskaltrust_Connection_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new FiskaltrustConnection());
        }
    }
}
