using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Threading;

namespace GoMusic.MyControls
{
    public partial class SplashScreenControl : UserControl
    {
        #region HeightProperty
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("ItemsSource", typeof(double), typeof(SplashScreenControl), new PropertyMetadata(default(double), HeightPropertyChanged));

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplashScreenControl myControl = d as SplashScreenControl;
            var newValue = (double)e.NewValue;
            myControl.LayoutRoot.Height = newValue;
        }

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        #endregion
        #region WidthProperty
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("ItemsSource", typeof(double), typeof(SplashScreenControl), new PropertyMetadata(default(double), WidthPropertyChanged));

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplashScreenControl myControl = d as SplashScreenControl;
            var newValue = (double)e.NewValue;
            myControl.LayoutRoot.Width = newValue;
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        #endregion
        //#region LoadingProgressProperty
        //public static readonly DependencyProperty LoadingProgressProperty =
        //    DependencyProperty.Register("ItemsSource", typeof(double), typeof(SplashScreenControl), new PropertyMetadata(default(double), LoadingProgressPropertyChanged));

        //private static void LoadingProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    SplashScreenControl myControl = d as SplashScreenControl;
        //    var newValue = (double)e.NewValue;
        //    myControl.textBlock1.Text = "Loading " + newValue.ToString();
        //}

        //public double LoadingProgress
        //{
        //    get { return (double)GetValue(LoadingProgressProperty); }
        //    set { SetValue(LoadingProgressProperty, value); }
        //}
        //#endregion
        public double LoadingProgress
        {
            set { if (pReport != null) pReport.Progress = value; }
        }

        public ProgressReport pReport;

        //DispatcherTimer timer;

        public SplashScreenControl()
        {
            InitializeComponent();

            pReport = new ProgressReport();

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(200);
            //timer.Tick += timer_Tick;
            //timer.Start();
            (Resources["Loading2Circle"] as System.Windows.Media.Animation.Storyboard).Begin();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    textBlock1.Text = GoMusic.Services.GeneralService.InitPercent.ToString();
            //}
            //catch (Exception) { }
        }
    }

    public class ProgressReport : INotifyPropertyChanged
    {
        private double progress;

        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public ProgressReport()
        {
            progress = 2;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
