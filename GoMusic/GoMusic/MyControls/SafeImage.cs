using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.MyControls
{
    /// <summary>
    /// Image control that auto release memory when unloaded
    /// </summary>
    public class SafeImage : System.Windows.Controls.ContentControl
    {
        public SafeImage()
        {
            this.Unloaded += SafeImage_Unloaded;
        }

        void SafeImage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var image = this.Content as System.Windows.Controls.Image;
            if (image != null)
            {
                var bitmap = image.Source as System.Windows.Media.Imaging.BitmapSource;
                if (bitmap != null)
                    bitmap.SetSource(null);
                image.Source = null;
            }
        }
    }
}
