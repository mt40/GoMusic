using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GoMusic.Converters
{
    public class StreamToImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var getArt = value as GoMusic.Models.GetAlbumArtDelegate;
            if (getArt == null) return null;
            using (var imageStream = getArt())
            {
                if (null != imageStream)
                {
                    try
                    {
                        var rs = Microsoft.Phone.PictureDecoder.DecodeJpeg(imageStream, 120, 120);

                        return rs;
                    }
                    catch
                    {
                        //return "/Assets/Images/cover_dark.png";

                        return null;
                    }

                }
                //return "/Assets/Images/cover_dark.png";
                return null;
            }
            //using (var imageStream = value as System.IO.Stream)
            //{
            //    if (null != imageStream)
            //    {
            //        try
            //        {
            //            var rs = Microsoft.Phone.PictureDecoder.DecodeJpeg(imageStream, 120, 120);

            //            return rs;
            //        }
            //        catch
            //        {
            //            //return "/Assets/Images/cover_dark.png";

            //            return null;
            //        }

            //    }
            //    //return "/Assets/Images/cover_dark.png";
            //    return null;
            //}
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
