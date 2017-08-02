using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifViewer.Rendering
{
    public class BlockDecoder : IConsumer<string>
    {
        private IConsumer<string> next;

        private string accumulator;

        public BlockDecoder(IConsumer<string> next)
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
}
