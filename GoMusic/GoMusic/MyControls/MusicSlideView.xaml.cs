using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Xna.Framework.Media;

namespace GoMusic.MyControls
{
    public partial class MusicSlideView : UserControl
    {
        #region ItemsSourceProperty
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MusicSlideView), new PropertyMetadata(default(IEnumerable), ItemsSourcePropertyChanged));

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MusicSlideView myControl = d as MusicSlideView;
            var newValue = (e.NewValue as IEnumerable).Cast<object>().ToList();
            
            if (newValue.Count < 3)
                throw new ArgumentException("ItemsSource must have more than 3 items!");
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        public delegate void ValueChangedEventHandler(object sender, EventArgs e);

        public event ValueChangedEventHandler MoveNext;
        public event ValueChangedEventHandler MovePrevious;

        public MusicSlideView()
        {
            InitializeComponent();
            
            source = new List<Song>();
            source2 = new List<int>();
            NullImage = new BitmapImage(new Uri("/Assets/Images/cover.png", UriKind.RelativeOrAbsolute));

            Loaded += MusicSlideView_Loaded;
        }

        private BitmapImage NullImage;
        private List<Song> source;
        private List<int> source2;

        void MusicSlideView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource == null)
                return;
            source = ItemsSource as List<Song>;
            for (int i = 0; i < source.Count; i++)
                source2.Add(i);

            var stream1 = source[source.Count - 1].Album.GetAlbumArt();
            var stream2 = source[0].Album.GetAlbumArt();
            var stream3 = source[1].Album.GetAlbumArt();
            if (stream1 != null)
                ImageLeft.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(stream1);
            else
                ImageLeft.Source = NullImage;
            if (stream2 != null)
                ImageMid.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(stream2);
            else
                ImageMid.Source = NullImage;
            if (stream3 != null)
                ImageRight.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(stream3);
            else
                ImageRight.Source = NullImage;

            //TextBlockLeft.Text = source2[source2.Count - 1].ToString();
            //TextBlockMid.Text = "0";
            //TextBlockRight.Text = "1";
        }
       
        private void MoveItemRight()
        {
            var left = LayoutRoot.Children[0];
            var mid = LayoutRoot.Children[2];
            var right = LayoutRoot.Children[1];
            LayoutRoot.Children.RemoveAt(0);
            LayoutRoot.Children.Add(left);

            var moveRight = (Storyboard)canvas.Resources["MoveRight"];
            moveRight.Completed += (s, e) => moveRight_Completed(left, mid, right);
            moveRight.SkipToFill();
            moveRight.Stop();
            //setup Canvas.Left animation
            ((DoubleAnimation)moveRight.Children[0]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[1]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[2]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Canvas.Top animation
            ((DoubleAnimation)moveRight.Children[3]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[4]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[5]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Width animation
            ((DoubleAnimation)moveRight.Children[6]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[7]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[8]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Height animation
            ((DoubleAnimation)moveRight.Children[9]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[10]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveRight.Children[11]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            moveRight.Begin();
        }

        void moveRight_Completed(UIElement left, UIElement mid, UIElement right)
        {
            mid.SetValue(Canvas.TopProperty, 75.0);
            left.SetValue(Canvas.TopProperty, 50.0);
            right.SetValue(Canvas.TopProperty, 75.0);

            (mid as Grid).Height = 200;
            (mid as Grid).Width = 200;
            (right as Grid).Height = 200;
            (right as Grid).Width = 200;
            (left as Grid).Height = 250;
            (left as Grid).Width = 250;
        }

        private void MoveItemLeft()
        {
            var left = LayoutRoot.Children[0];
            var mid = LayoutRoot.Children[2];
            var right = LayoutRoot.Children[1];         
            LayoutRoot.Children.RemoveAt(1);
            LayoutRoot.Children.RemoveAt(0);
            LayoutRoot.Children.Add(left);
            LayoutRoot.Children.Add(right);

            var moveLeft = (Storyboard)canvas.Resources["MoveLeft"];
            moveLeft.Completed += (s, e) => moveLeft_Completed(left, mid, right);
            moveLeft.SkipToFill();
            moveLeft.Stop();
            //setup Canvas.Left animation
            ((DoubleAnimation)moveLeft.Children[0]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[1]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[2]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Canvas.Top animation
            ((DoubleAnimation)moveLeft.Children[3]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[4]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[5]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Width animation
            ((DoubleAnimation)moveLeft.Children[6]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[7]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[8]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            //setup Height animation
            ((DoubleAnimation)moveLeft.Children[9]).SetValue(Storyboard.TargetNameProperty, (left as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[10]).SetValue(Storyboard.TargetNameProperty, (mid as FrameworkElement).Name);
            ((DoubleAnimation)moveLeft.Children[11]).SetValue(Storyboard.TargetNameProperty, (right as FrameworkElement).Name);
            moveLeft.Begin();

        }

        void moveLeft_Completed(UIElement left, UIElement mid, UIElement right)
        {
            mid.SetValue(Canvas.TopProperty, 75.0);
            left.SetValue(Canvas.TopProperty, 75.0);
            right.SetValue(Canvas.TopProperty, 50.0);

            (mid as Grid).Height = 200;
            (mid as Grid).Width = 200;
            (right as Grid).Height = 250;
            (right as Grid).Width = 250;
            (left as Grid).Height = 200;
            (left as Grid).Width = 200;
        }

        double initialPosition;
        private void canvas_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (ItemsSource == null)
                return;

            initialPosition = (double)LayoutRoot.Children[2].GetValue(Canvas.LeftProperty);
        }

        private void canvas_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (ItemsSource == null)
                return;

            var mid = LayoutRoot.Children[2] as FrameworkElement;
            if (e.DeltaManipulation.Translation.X != 0)
            {
                var offset = Math.Min(Math.Max(50, (double)mid.GetValue(Canvas.LeftProperty) + e.DeltaManipulation.Translation.X), 200);
                mid.SetValue(Canvas.LeftProperty, offset);
            }
        }

        private void canvas_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (ItemsSource == null)
                return;
 
            var leftOffset = (double)LayoutRoot.Children[2].GetValue(Canvas.LeftProperty);

            if (Math.Abs(initialPosition - leftOffset) < 50)
            {
                //bouncing back
                var mid = LayoutRoot.Children[2] as FrameworkElement;
                mid.SetValue(Canvas.LeftProperty, 100.0);
                return;
            }
            //change of state
            if (initialPosition - leftOffset > 0)
            {
                //slide to the left                
                Grid left = LayoutRoot.Children[0] as Grid;
                Image imageLeft = left.Children[0] as Image;  
                        
                var top = source[0];
                source.RemoveAt(0);
                source.Insert(source.Count - 1, top);               

                //BitmapImage bitmapImage = imageLeft.Source as BitmapImage;
                //bitmapImage.UriSource = null;
                imageLeft.Source = null;
                var stream = source[1].Album.GetAlbumArt();
                if (stream != null)
                    imageLeft.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(stream);
                else
                    imageLeft.Source = NullImage;

                //var top2 = source2[0];
                //source2.RemoveAt(0);
                //source2.Insert(source2.Count, top2);
                ////(left.Children[1] as TextBlock).Text = source2[1].ToString();
                //(left.Children[1] as TextBlock).Text = source[1].Name;

                MoveItemLeft();

                //trigger event
                if(MoveNext != null)
                {
                    MoveNext(this, EventArgs.Empty);
                }
            }
            else
            {
                //slide to the right
                Grid right = LayoutRoot.Children[1] as Grid;
                Image imageRight = right.Children[0] as Image;

                var bottom = source[source.Count - 1];
                source.RemoveAt(source.Count - 1);
                source.Insert(0, bottom);

                imageRight.Source = null;
                var stream = source[source.Count - 1].Album.GetAlbumArt();
                if (stream != null)
                    imageRight.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(stream);
                else
                    imageRight.Source = NullImage;

                //var bottom2 = source2[source2.Count - 1];
                //source2.RemoveAt(source2.Count - 1);
                //source2.Insert(0, bottom2);
                ////(right.Children[1] as TextBlock).Text = source2[source2.Count - 1].ToString();
                //(right.Children[1] as TextBlock).Text = source[source.Count - 1].Name;
                MoveItemRight();

                //trigger event
                if(MovePrevious != null)
                {
                    MovePrevious(this, EventArgs.Empty);
                }
            }
        }
    
    }
}
