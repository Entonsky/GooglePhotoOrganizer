using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GooglePhotoOrganizer
{
    class ImageWorker
    {

        public static string ExifDateToGoogleFormat(string date)
        {
            DateTime dateParsed;
            if (DateTime.TryParse(date, out dateParsed))
                date = dateParsed.ToString("yyyy:MM:dd HH:mm:ss");
            return date;
        }


        private static bool IsImage(FileStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return true;
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return false;
        }
        

        public static string GetExifDateInGoogleFormat(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!IsImage(fs))
                return "";
            try
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                string date = md.DateTaken;
                return ExifDateToGoogleFormat(date);
            }
            catch
            {
                return "";
            }
        }

    }
}
