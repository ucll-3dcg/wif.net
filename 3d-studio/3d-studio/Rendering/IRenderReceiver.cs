using System;
using System.Collections.Generic;
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
}
