﻿#pragma checksum "D:\Minh Thái\My Documents\University HCMUS\WP8 Apps\GoMusic\GoMusic\GoMusic\Views\DarkTheme\SettingPageDark.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C22DA45AB0286D4F679DB8E1EEED2309"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GoMusic.Controls;
using GoMusic.MyControls;
using GoogleAds;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telerik.Windows.Controls;


namespace GoMusic.Views.DarkTheme {
    
    
    public partial class SettingPageDark : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard StoryboardSongCount;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal GoogleAds.AdView adDuplexAd;
        
        internal System.Windows.Controls.Grid grid_MainContent;
        
        internal System.Windows.Controls.Grid GridStatus;
        
        internal System.Windows.Controls.TextBlock tb_Status;
        
        internal System.Windows.Controls.Grid grid_Theme;
        
        internal System.Windows.Controls.ListBox ImageBar;
        
        internal System.Windows.Controls.ListBoxItem ListBoxSetting;
        
        internal GoMusic.Controls.PathControl PathSetting;
        
        internal System.Windows.Controls.ListBoxItem ListBoxInfo;
        
        internal GoMusic.Controls.PathControl PathInfo;
        
        internal Microsoft.Phone.Controls.Pivot PivotHome;
        
        internal Telerik.Windows.Controls.RadListPicker radListPicker;
        
        internal GoMusic.MyControls.MySwitch SwitchSleepTimer;
        
        internal Telerik.Windows.Controls.RadTimeSpanPicker radTimeSpanPicker;
        
        internal GoMusic.MyControls.MySwitch SwitchListView;
        
        internal GoMusic.MyControls.MySwitch SwitchLyrics;
        
        internal System.Windows.Controls.Button ButtonTheme;
        
        internal System.Windows.Controls.Button ButtonBuy;
        
        internal System.Windows.Controls.Border BorderPro;
        
        internal System.Windows.Controls.TextBlock TextBlockSongCount;
        
        internal System.Windows.Controls.TextBlock TextBlockArtistCount;
        
        internal System.Windows.Controls.TextBlock TextBlockAlbumCount;
        
        internal System.Windows.Controls.TextBlock TextBlockPlaylistCount;
        
        internal Telerik.Windows.Controls.RadListPicker radListPickerLanguage;
        
        internal System.Windows.Controls.Button ButtonClearLyrics;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/GoMusic;component/Views/DarkTheme/SettingPageDark.xaml", System.UriKind.Relative));
            this.StoryboardSongCount = ((System.Windows.Media.Animation.Storyboard)(this.FindName("StoryboardSongCount")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.adDuplexAd = ((GoogleAds.AdView)(this.FindName("adDuplexAd")));
            this.grid_MainContent = ((System.Windows.Controls.Grid)(this.FindName("grid_MainContent")));
            this.GridStatus = ((System.Windows.Controls.Grid)(this.FindName("GridStatus")));
            this.tb_Status = ((System.Windows.Controls.TextBlock)(this.FindName("tb_Status")));
            this.grid_Theme = ((System.Windows.Controls.Grid)(this.FindName("grid_Theme")));
            this.ImageBar = ((System.Windows.Controls.ListBox)(this.FindName("ImageBar")));
            this.ListBoxSetting = ((System.Windows.Controls.ListBoxItem)(this.FindName("ListBoxSetting")));
            this.PathSetting = ((GoMusic.Controls.PathControl)(this.FindName("PathSetting")));
            this.ListBoxInfo = ((System.Windows.Controls.ListBoxItem)(this.FindName("ListBoxInfo")));
            this.PathInfo = ((GoMusic.Controls.PathControl)(this.FindName("PathInfo")));
            this.PivotHome = ((Microsoft.Phone.Controls.Pivot)(this.FindName("PivotHome")));
            this.radListPicker = ((Telerik.Windows.Controls.RadListPicker)(this.FindName("radListPicker")));
            this.SwitchSleepTimer = ((GoMusic.MyControls.MySwitch)(this.FindName("SwitchSleepTimer")));
            this.radTimeSpanPicker = ((Telerik.Windows.Controls.RadTimeSpanPicker)(this.FindName("radTimeSpanPicker")));
            this.SwitchListView = ((GoMusic.MyControls.MySwitch)(this.FindName("SwitchListView")));
            this.SwitchLyrics = ((GoMusic.MyControls.MySwitch)(this.FindName("SwitchLyrics")));
            this.ButtonTheme = ((System.Windows.Controls.Button)(this.FindName("ButtonTheme")));
            this.ButtonBuy = ((System.Windows.Controls.Button)(this.FindName("ButtonBuy")));
            this.BorderPro = ((System.Windows.Controls.Border)(this.FindName("BorderPro")));
            this.TextBlockSongCount = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlockSongCount")));
            this.TextBlockArtistCount = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlockArtistCount")));
            this.TextBlockAlbumCount = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlockAlbumCount")));
            this.TextBlockPlaylistCount = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlockPlaylistCount")));
            this.radListPickerLanguage = ((Telerik.Windows.Controls.RadListPicker)(this.FindName("radListPickerLanguage")));
            this.ButtonClearLyrics = ((System.Windows.Controls.Button)(this.FindName("ButtonClearLyrics")));
        }
    }
}
