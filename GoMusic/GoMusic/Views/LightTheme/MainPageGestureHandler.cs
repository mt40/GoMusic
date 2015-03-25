using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System;
using System.Windows;
using GoMusic.Framework;
using GoMusic.Services;

namespace GoMusic.Views.LightTheme
{
    public partial class MainPage : PhoneApplicationPage
    {       
        private void grid_Tray_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.TotalManipulation.Translation.Y < -40 && isTrayVolumeOpened == false)
            {
                VisualStateManager.GoToState(this, "TrayVolumeOpened", true);
                isTrayVolumeOpened = true;
            }
        }

        private void ButtonTrayVolumeClose_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TrayVolumeClosed", true);
            isTrayVolumeOpened = false;
        }

    }
}