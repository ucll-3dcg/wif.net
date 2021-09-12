using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WifViewer
{
    public class MovieExporter
    {
        public static void Export(List<WriteableBitmap> frames, string outputFile)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = $"-y -framerate 30 -vcodec png -i - -c:v libx264 -r 30 -pix_fmt yuv420p {outputFile}"
            };

            process.Start();

            using (var binaryWriter = new BinaryWriter(process.StandardInput.BaseStream))
            {
                foreach (var frame in frames)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(frame));

                    var memoryStream = new MemoryStream();
                    encoder.Save(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    binaryWriter.Write(buffer, 0, buffer.Length);
                }
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Debug.WriteLine(output);
        }
    }
}
