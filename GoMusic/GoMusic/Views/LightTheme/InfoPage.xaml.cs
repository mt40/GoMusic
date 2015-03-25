using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace GoMusic.Views.LightTheme
{
    public partial class InfoPage : PhoneApplicationPage
    {
        public InfoPage()
        {
            InitializeComponent();

            //grid_Theme.DataContext = GoMusic.Services.ThemeService.Instance;

            //musicSlideView.ItemsSource = GoMusic.Services.MediaService.Instance.SongList;
        }

        private void ButtonContact_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "GoMusic Suggestion";
            emailComposeTask.To = "GoMusic@mailinator.com";

            emailComposeTask.Show();
        }
    }
}