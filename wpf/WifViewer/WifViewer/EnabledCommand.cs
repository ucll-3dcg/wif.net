using Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WifViewer
{
    public class EnabledCommand : CellCommand
    {
        public EnabledCommand(Action action)
            : base(Cell.Create(true), action)
        {
            // NOP
        }
    }

    public class EnabledCommand<T> : CellCommand<T>
    {
        public EnabledCommand(Action<T> action)
            : base(Cell.Create(false), action)
        {
            // NOP
        }
    }
}
