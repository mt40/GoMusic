using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GoMusic.Resources;
using GoMusic.Services;
using Telerik.Windows.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using Microsoft.Phone.Marketplace;

namespace GoMusic
{
    public partial class App : Application
    {
        private static LicenseInformation _licenseInfo = new LicenseInformation();
        private static bool _isTrial = true;
        private int i = 4;
        public bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            //hocket app api for reporting crash            
            //HockeyApp.CrashHandler.Instance.Configure(this, "3531f66f8506584d3c621ffe10759451", RootFrame);

            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();
            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {

                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //ApplicationUsageHelper.Init("1.7");
            GeneralService.Instance.IsFirstLaunch = true;
            var sv = ThemeService.Instance;
            ThemeService.LoadTheme();
            ThemeService.BeginTheme();
            try
            {
                //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                MediaService.Instance.CurrentSong = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong;

                ((System.Windows.Media.SolidColorBrush)App.Current.Resources["PhoneForegroundBrush"]).Color
                    = System.Windows.Media.Colors.White;
            }
            catch { }
            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            if (GeneralService.Instance.IsDarkThemeUsed)
                brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 40, 40, 46));
            RootFrame.Background = brush;

            CheckLicense();
        }

        void InitLibrary()
        {
            try
            {
                var tmp = MediaService.Instance.SongList;
                var tmp2 = MediaService.Instance.ArtistList;
                var tmp3 = MediaService.Instance.AlbumList;
            }
            catch { }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //ApplicationUsageHelper.OnApplicationActivated();
            GeneralService.Instance.IsReactivated = true;
            try
            {
                //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                MediaService.Instance.CurrentSong = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong;
            }
            catch
            {
                MediaService.Instance.CurrentSong = null;
            }

            //CheckLicense();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            try
            {
                GeneralService.CreateAndUpdateLiveTile2();
            }
            catch { }
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            GeneralService.Instance.UseCount += 1;
            try { GeneralService.SaveRecentQuery(); }
            catch { }
            try { GeneralService.CreateAndUpdateLiveTile2(); }
            catch { }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RadPhoneApplicationFrame frame = new RadPhoneApplicationFrame()
            {
                Transition = new RadSlideTransition()
                    {
                        Orientation = System.Windows.Controls.Orientation.Vertical
                    }
            };
            RootFrame = frame;

            RootFrame.Navigated += CompleteInitializePhoneApplication;
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
            
            var service = GeneralService.Instance;
            //choose start page
            if (service.IsDarkThemeUsed)
            {
                if (service.IsListViewUsed)
                    RootFrame.Navigate(new Uri("/Views/DarkTheme/MainPageDark_List.xaml", UriKind.RelativeOrAbsolute));
                else
                    RootFrame.Navigate(new Uri("/Views/DarkTheme/MainPageDark.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                if (service.IsListViewUsed)
                    RootFrame.Navigate(new Uri("/Views/LightTheme/MainPage_List.xaml", UriKind.RelativeOrAbsolute));
                else
                    RootFrame.Navigate(new Uri("/Views/LightTheme/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        private void InitializeLanguage()
        {
            try
            {
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        private void CheckLicense()
        {
            // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
            // property to simulate trial mode being on or off. 
#if DEBUG
            _isTrial = true;

#else
            _isTrial = _licenseInfo.IsTrial();
#endif
        }
    }
}