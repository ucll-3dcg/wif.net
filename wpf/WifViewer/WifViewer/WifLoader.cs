using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WifViewer
{
    public class WifLoader
    {
        public static List<WriteableBitmap> Load(string path)
        {
            var result = new List<WriteableBitmap>();

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                while (true)
                {
                    var buffer = new byte[4];

                    fileStream.Read(buffer, 0, 4);
                    var width = BitConverter.ToUInt32(buffer, 0);

                    if (width == 0)
                        return result;

                    fileStream.Read(buffer, 0, 4);
                    var height = BitConverter.ToUInt32(buffer, 0);

                    buffer = new byte[3 * width * height];
                    fileStream.Read(buffer, 0, buffer.Length);

                    var bitmap = new WriteableBitmap((int) width, (int) height, 96, 96, PixelFormats.Rgb24, null);
                    var rect = new Int32Rect(0, 0, (int)width, (int)height);
                    bitmap.WritePixels(rect, buffer, (int) width * 3, 0);

                    result.Add(bitmap);
                }
            }
        }
    }
}
