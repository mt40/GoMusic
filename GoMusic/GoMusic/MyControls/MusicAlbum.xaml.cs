using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GoMusic.MyControls
{
    public delegate void AnimationCompleteEventHandler(object sender, EventArgs e);

    public partial class MusicAlbum : UserControl
    {
        #region FirstItemImageSource Property
        public static readonly DependencyProperty FirstItemImageSourceProperty =
            DependencyProperty.Register("FirstItemImageSource ", typeof(ImageSource), typeof(MusicAlbum), new PropertyMetadata(default(ImageSource), FirstItemImageSourcePropertyChanged));

        private static void FirstItemImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MusicAlbum myControl = d as MusicAlbum;
            var newValue = (ImageSource)e.NewValue;
            var ui = myControl.FindName("grid" + myControl._firstItem) as Grid;
            var img = ui.Children[1] as Image;
            var ui2 = (myControl.FindName("grid" + myControl._secondItem) as Grid).Children[1] as Image;
            var ui3 = (myControl.FindName("grid" + myControl._thirdItem) as Grid).Children[1] as Image;
            ClearImageSource(ref ui3);
            ClearImageSource(ref ui2);

            img.Source = newValue;
        }

        public ImageSource FirstItemImageSource
        {
            get { return (ImageSource)GetValue(FirstItemImageSourceProperty); }
            set { SetValue(FirstItemImageSourceProperty, value); }
        }
        #endregion

        #region ItemBackgroundProperty
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(MusicAlbum), new PropertyMetadata(default(Brush), ItemBackgroundPropertyChanged));

        private static void ItemBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MusicAlbum myControl = d as MusicAlbum;
            var newValue = e.NewValue as Brush;
            if (newValue != null)
            {
                myControl.grid0.Background = newValue;
                myControl.grid1.Background = newValue;
                myControl.grid2.Background = newValue;
            }
        }

        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        #endregion

        private int _firstItem, _secondItem, _thirdItem;

        public event AnimationCompleteEventHandler AnimationComplete;

        public MusicAlbum()
        {
            InitializeComponent();

            _firstItem = 0;
            _secondItem = 1;
            _thirdItem = 2;
        }

        /// <summary>
        /// Move to the next item.
        /// </summary>
        public void ToNextItem()
        {
            NextItem.Begin();
        }

        public void ToPreviousItem()
        {
            PreviousItem.Begin();
        }

        private void NextItem_Completed(object sender, EventArgs e)
        {
            //stop the storyboard first
            UpdateItemProperty("grid0");
            UpdateItemProperty("grid1");
            UpdateItemProperty("grid2");

            (sender as Storyboard).Stop();
            UpdateOrder();
            var animations = NextItem.Children;
            for (int i = 0; i < animations.Count; i++)
            {
                if (i < 5)
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _firstItem.ToString());
                }
                else if (i < 9)
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _secondItem.ToString());
                }
                else
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _thirdItem.ToString());
                }
            }

            if (AnimationComplete != null)
                AnimationComplete(this, EventArgs.Empty);
        }

        private void PreviousItem_Completed(object sender, EventArgs e)
        {
            //stop the storyboard first
            UpdateItemProperty("grid0");
            UpdateItemProperty("grid1");
            UpdateItemProperty("grid2");

            (sender as Storyboard).Stop();
            UpdateOrder(isReverse:true);
            var animations = NextItem.Children;
            for (int i = 0; i < animations.Count; i++)
            {
                if (i < 5)
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _firstItem.ToString());
                }
                else if (i < 9)
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _secondItem.ToString());
                }
                else
                {
                    animations[i].SetValue(Storyboard.TargetNameProperty, "grid" + _thirdItem.ToString());
                }
            }

            if (AnimationComplete != null)
                AnimationComplete(this, EventArgs.Empty);
        }

        private void UpdateOrder(bool isReverse = false)
        {
            if (isReverse)
            {
                _firstItem = CircleNumber(_firstItem - 1, 0, 2);
                _secondItem = CircleNumber(_secondItem - 1, 0, 2);
                _thirdItem = CircleNumber(_thirdItem - 1, 0, 2);
            }
            else
            {
                _firstItem = CircleNumber(_firstItem + 1, 0, 2);
                _secondItem = CircleNumber(_secondItem + 1, 0, 2);
                _thirdItem = CircleNumber(_thirdItem + 1, 0, 2);
            }
        }

        /// <summary>
        /// Return 'min' if 'num' exceeds 'max' and vice versa
        /// </summary>
        private int CircleNumber(int num, int min, int max)
        {
            if (num > max)
                return min;
            if (num < min)
                return max;
            return num;
        }

        private void UpdateItemProperty(string element_name)
        {
            //we need to save all the transforms when the storyboard completed
            //it will be discarded when Stop() is called
            var ui = this.FindName(element_name) as UIElement;
            Canvas.SetZIndex(ui, Canvas.GetZIndex(ui));
            var transform = ui.RenderTransform as CompositeTransform;
            double y = transform.TranslateY;
            double scale_x = transform.ScaleX;
            double scale_y = transform.ScaleY;

            transform.SetValue(CompositeTransform.TranslateYProperty, y);
            transform.SetValue(CompositeTransform.ScaleXProperty, scale_x);
            transform.SetValue(CompositeTransform.ScaleYProperty, scale_y);
        }

        private static void ClearImageSource(ref Image img)
        {
            var bitmap = img.Source as System.Windows.Media.Imaging.BitmapImage;
            //var writeableBitmap = img.Source as System.Windows.Media.Imaging.WriteableBitmap;
            if (bitmap != null)
            {
                bitmap.UriSource = null;
            }
            img.Source = null;
        }

        
    }
}
