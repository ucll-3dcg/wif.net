using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifViewer.Rendering
{
    public interface IConsumer<T>
    {
        void Consume(T t);
    }
}
