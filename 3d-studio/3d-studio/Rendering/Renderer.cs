using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WifViewer.Rendering
{
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

    public class Renderer
    {
        public void Render(string script, IRenderReceiver receiver)
        {
            var bitmapDecoder = new BitmapDecoder(new RenderReceiverAdapter(receiver));
            var base64Decoder = new Base64Decoder(bitmapDecoder);
            var blockDecoder = new BlockDecoder(base64Decoder);

            this.process = new ExternalProcess()
            {
                ExecutablePath = Configuration.RAYTRACER_PATH,
                CommandLineArguments = "-s -",
                Input = script,
                OnOutputDataReceived = blockDecoder.Consume,
                OnErrorDataReceived = receiver.Message,
                OnExited = receiver.RenderingDone
            };

            this.process.Start();
        }

        private ExternalProcess process;

        public void Stop()
        {
            process.Stop();
        }
    }
}
