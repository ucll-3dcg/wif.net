using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WifViewer.Rendering;

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

                    var bitmap = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Rgb24, null);
                    var rect = new Int32Rect(0, 0, (int)width, (int)height);
                    bitmap.WritePixels(rect, buffer, (int)width * 3, 0);

                    result.Add(bitmap);
                }
            }
        }

        public static void Load(string path, IRenderReceiver receiver)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                while (true)
                {
                    var buffer = new byte[4];

                    fileStream.Read(buffer, 0, 4);
                    var width = BitConverter.ToUInt32(buffer, 0);

                    if (width == 0)
                    {
                        break;
                    }

                    fileStream.Read(buffer, 0, 4);
                    var height = BitConverter.ToUInt32(buffer, 0);

                    buffer = new byte[3 * width * height];
                    fileStream.Read(buffer, 0, buffer.Length);

                    var bitmap = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Rgb24, null);
                    var rect = new Int32Rect(0, 0, (int)width, (int)height);
                    bitmap.WritePixels(rect, buffer, (int)width * 3, 0);
                    bitmap.Freeze();

                    receiver.FrameRendered(bitmap);
                }

                receiver.RenderingDone();
            }
        }

        public static void LoadInSeparateThread(string path, IRenderReceiver receiver)
        {
            var thread = new Thread(() => Load(path, receiver));
            thread.Start();
        }

        public static WriteableBitmap DecodeFrame(byte[] buffer)
        {
            var width = BitConverter.ToUInt32(buffer, 0);

            if (width == 0)
                return null;

            var height = BitConverter.ToUInt32(buffer, 4);

            var copy = new byte[width * height * 3];
            Array.Copy(buffer, 8, copy, 0, copy.Length);

            var bitmap = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Rgb24, null);
            var rect = new Int32Rect(0, 0, (int)width, (int)height);
            bitmap.WritePixels(rect, copy, (int)width * 3, 0);

            bitmap.Freeze();

            return bitmap;
        }
    }
}
