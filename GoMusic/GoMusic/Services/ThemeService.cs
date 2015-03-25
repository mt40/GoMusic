using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GoMusic.Services
{
    public sealed class ThemeService : INotifyPropertyChanged
    {
        //this service provides methods for theme color

        private static readonly ThemeService instance = new ThemeService();

        private ThemeService()
        {
            _accentColor = Color.FromArgb(255, 255, 56, 56);
            _themeName = "Red";
            _currentThemeType = ThemesType.Light;
            ((SolidColorBrush)App.Current.Resources["PhoneAccentBrush"]).Color = Color.FromArgb(255, 255, 56, 56);
        }

        public static ThemeService Instance
        {
            get { return instance; }
        }

        private string _themeName;

        public string ThemeName
        {
            get { return _themeName; }
            set { _themeName = value; }
        }

        private ThemesType _currentThemeType;

        public ThemesType CurrentThemeType
        {
            get { return _currentThemeType; }
            set { _currentThemeType = value; }
        }

        private Color _accentColor;

        public Color AccentColor
        {
            get
            {
                return _accentColor;
            }
            set
            {
                _accentColor = value;

                NotifyPropertyChanged("AccentColor");
                NotifyPropertyChanged("DarkAccentColor");
                NotifyPropertyChanged("BindingAccentColor");
            }           
        }

        public Color DarkAccentColor
        {
            get
            {
                return (LightenDarkenColor(_accentColor, -0.3f));
            }
        }

        public SolidColorBrush BindingAccentColor
        {
            get
            {
                return new SolidColorBrush(_accentColor);
            }
            
        }

        /// <summary>
        /// Load the current selected theme
        /// </summary>
        public static void BeginTheme()
        {
            ((SolidColorBrush)App.Current.Resources["PhoneAccentBrush"]).Color = ThemeService.Instance._accentColor;
        }

        /// <summary>
        /// Lighten or darken color from color code
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="factor">Factor from 0 to 1. Positive = lighten. Negative = darken</param>
        public Color LightenDarkenColor(Color color, float factor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (factor < 0)
            {
                factor = 1 + factor;
                red *= factor;
                green *= factor;
                blue *= factor;
            }
            else
            {
                red = (255 - red) * factor + red;
                green = (255 - green) * factor + green;
                blue = (255 - blue) * factor + blue;
            }

            return Color.FromArgb(color.A, (byte)(int)red, (byte)(int)green, (byte)(int)blue);
        }

        /// <summary>
        /// Save theme settings to IsolatedStorage
        /// </summary>
        public static void SaveTheme()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["ThemeName"] = ThemeService.instance._themeName;
            settings["ThemeType"] = ThemeService.instance._currentThemeType;
            settings["AccentColor"] = ThemeService.instance._accentColor;
            settings.Save();
        }

        /// <summary>
        /// Load theme setttings from IsolatedStorage
        /// </summary>
        public static void LoadTheme()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("AccentColor"))
                ThemeService.instance._accentColor = (Color)settings["AccentColor"];
            if (settings.Contains("ThemeName"))
                ThemeService.instance._themeName = (string)settings["ThemeName"];
            if (settings.Contains("ThemeType"))
                ThemeService.instance._currentThemeType = (ThemesType)settings["ThemeType"];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum ThemesType
    {
        Light, Dark, LightSquare, DarkSquare
    }
}
