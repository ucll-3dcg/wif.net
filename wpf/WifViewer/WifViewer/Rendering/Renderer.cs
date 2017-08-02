using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WifViewer.Rendering
{
    public interface IRenderReceiver
    {
        void FrameRendered(WriteableBitmap frame);

        void RenderingDone();

        void Message(string message);
    }

    public class RenderReceiverAdapter : IConsumer<WriteableBitmap>
    {
        private IRenderReceiver receiver;

        public RenderReceiverAdapter(IRenderReceiver receiver)
        {
            this.receiver = receiver;
        }

        public void Consume(WriteableBitmap bitmap)
        {
            receiver.FrameRendered(bitmap);
        }
    }

    public class ExternalProcess
    {
        public string ExecutablePath { get; set; }

        public string CommandLineArguments { get; set; }

        public string Input { get; set; }

        public Action<string> OnOutputDataReceived { get; set; }

        public Action<string> OnErrorDataReceived { get; set; }

        public Action OnExited { get; set; }


        public void Start()
        {
            var process = new Process();
            process.StartInfo.FileName = this.ExecutablePath;
            process.StartInfo.Arguments = this.CommandLineArguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, e) => OnOutputDataReceived?.Invoke(e.Data);
            process.ErrorDataReceived += (sender, e) => OnErrorDataReceived?.Invoke(e.Data);
            process.Exited += (sender, e) => OnExited?.Invoke();

            process.Start();

            if (this.Input != null)
            {
                process.StandardInput.WriteLine(this.Input);
                process.StandardInput.Close();
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }

    public class Renderer
    {
        private readonly string raytracerPath;

        public Renderer()
        {
            this.raytracerPath = @"E:\repos\ucll\shared\scripts\raytracer.exe";
        }

        public void Render(string script, IRenderReceiver receiver)
        {
            var bitmapDecoder = new BitmapDecoder(new RenderReceiverAdapter(receiver));
            var base64Decoder = new Base64Decoder(bitmapDecoder);
            var blockDecoder = new BlockConsumer(base64Decoder);

            var process = new ExternalProcess()
            {
                ExecutablePath = this.raytracerPath,
                CommandLineArguments = "-s -",
                Input = script,
                OnOutputDataReceived = blockDecoder.Consume,
                OnErrorDataReceived = receiver.Message,
                OnExited = receiver.RenderingDone
            };

            process.Start();
        }
    }

    public interface IConsumer<T>
    {
        void Consume(T t);
    }

    public class BitmapDecoder : IConsumer<byte[]>
    {
        private IConsumer<WriteableBitmap> next;

        public BitmapDecoder(IConsumer<WriteableBitmap> next)
        {
            this.next = next;
        }

        public void Consume(byte[] bytes)
        {
            var bitmap = WifLoader.DecodeFrame(bytes);

            if (bitmap != null)
            {
                next.Consume(bitmap);
            }
        }
    }

    public class BlockConsumer : IConsumer<string>
    {
        private IConsumer<string> next;

        private string accumulator;

        public BlockConsumer(IConsumer<string> next)
        {
            this.accumulator = "";
            this.next = next;
        }

        public void Consume(string str)
        {
            if (str != null)
            {
                switch (str)
                {
                    case "<<<":
                        if (accumulator.Length != 0)
                        {
                            throw new Exception("Faulty input from raytracer process");
                        }
                        break;

                    case ">>>":
                        Produce();
                        break;

                    default:
                        accumulator += str.Trim();
                        break;
                }
            }
        }

        private void Produce()
        {
            var accumulated = accumulator;
            accumulator = "";

            next.Consume(accumulated);
        }
    }

    public class Base64Decoder : IConsumer<string>
    {
        private IConsumer<byte[]> next;

        public Base64Decoder(IConsumer<byte[]> next)
        {
            this.next = next;
        }

        public void Consume(string str)
        {
            var bytes = System.Convert.FromBase64String(str);

            next.Consume(bytes);
        }
    }
}
