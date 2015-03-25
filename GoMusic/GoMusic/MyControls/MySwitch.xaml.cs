using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace GoMusic.MyControls
{
    public partial class MySwitch : UserControl
    {
        public delegate void ValueChangedEventHandler(object sender, EventArgs e);

        public event ValueChangedEventHandler Switched;

        #region CoverBrushProperty
        public static readonly DependencyProperty CoverBrushProperty =
            DependencyProperty.Register("CoverBrush", typeof(Brush), typeof(MySwitch), new PropertyMetadata(default(Brush), CoverBrushPropertyChanged));

        private static void CoverBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MySwitch myControl = d as MySwitch;
            var newValue = (Brush)e.NewValue;
            myControl.PolygonCover.Fill = newValue;
            myControl.PolygonCover2.Fill = newValue;
        }

        public Brush CoverBrush
        {
            get { return (Brush)GetValue(CoverBrushProperty); }
            set { SetValue(CoverBrushProperty, value); }
        }
        #endregion

        private bool isSwitchOn = false;

        public bool IsSwitchOn
        {
            get { return isSwitchOn; }
            set
            {
                isSwitchOn = value;
                if (isSwitchOn == true)
                {
                    //slide to the right  
                    //MoveViewWindow(-40);
                    //rectangle.Visibility = System.Windows.Visibility.Visible;
                    SwichOn();
                }
                else
                {
                    //slide to the left  
                    //MoveViewWindow(0);
                    //rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    SwitchOff();
                }
            }
        }

        public MySwitch()
        {
            InitializeComponent();

            //rectangle.DataContext = GoMusic.Services.ThemeService.Instance;
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isSwitchOn == false)
            {
                //slide to the right  
                SwichOn();
                isSwitchOn = true;
                //rectangle.Visibility = System.Windows.Visibility.Visible;
                if (Switched != null)
                {
                    Switched(this, EventArgs.Empty);
                }

            }
            else
            {
                //slide to the left  
                SwitchOff();
                isSwitchOn = false;
                //rectangle.Visibility = System.Windows.Visibility.Collapsed;
                if (Switched != null)
                {
                    Switched(this, EventArgs.Empty);
                }
            }
        }

        private void SwitchOff()
        {
            PolygonCover.Opacity = 1;
            (Resources["MoveSwitchOff"] as Storyboard).SkipToFill();
            (Resources["MoveSwitchOff"] as Storyboard).Begin();           
        }

        private void SwichOn()
        {
            (Resources["MoveSwitchOn"] as Storyboard).SkipToFill();
            (Resources["MoveSwitchOn"] as Storyboard).Begin();
            
        }

        //double initialPosition;
        bool _viewMoved = false;
        void MoveViewWindow(double left)
        {
            _viewMoved = true;
            //((DoubleAnimation)((Storyboard)canvas.Resources["moveAnimation"]).Children[0]).To = left;
            //((Storyboard)canvas.Resources["moveAnimation"]).Begin();
        }

        private void MoveSwitchOn_Completed(object sender, EventArgs e)
        {
            PolygonCover.Opacity = 0;
        }

        //private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        //{
        //    _viewMoved = false;
        //    initialPosition = Canvas.GetLeft(grid);
        //}       

        //private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        //{
        //    if (e.HorizontalChange != 0)
        //        Canvas.SetLeft(grid, Math.Min(Math.Max(0, Canvas.GetLeft(grid) + e.HorizontalChange), 50));
        //}

        //private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        //{

        //    var left = Canvas.GetLeft(grid);
        //    if (_viewMoved)
        //        return;
        //    if (Math.Abs(initialPosition - left) < 10)
        //    {
        //        //bouncing back
        //        MoveViewWindow(initialPosition);
        //        return;
        //    }
        //    //change of state  
        //    if (initialPosition - left < 0)
        //    {
        //        if (isSwitchOn == false)
        //        {
        //            //slide to the right  
        //            MoveViewWindow(50);
        //            rectangle.Visibility = System.Windows.Visibility.Visible;
        //            isSwitchOn = true;
        //            if (Switched != null)
        //            {
        //                Switched(this, EventArgs.Empty);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (isSwitchOn == true)
        //        {
        //            //slide to the left  
        //            MoveViewWindow(0);
        //            rectangle.Visibility = System.Windows.Visibility.Collapsed;
        //            isSwitchOn = false;
        //            if (Switched != null)
        //            {
        //                Switched(this, EventArgs.Empty);
        //            }
        //        }
        //    }          
        //}

    }
}
