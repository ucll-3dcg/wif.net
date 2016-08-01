using Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WifViewer
{
    public class CellCommand : CellCommand<object>
    {
        public CellCommand(Cell<bool> isEnabled, Action action)
            : base(isEnabled, x => action() )
        {
            // NOP
        }        
    }

    public class CellCommand<T> : ICommand
    {
        private readonly Cell<bool> isEnabled;

        private readonly Action<T> action;

        public CellCommand(Cell<bool> isEnabled, Action<T> action)
        {
            this.isEnabled = isEnabled;
            this.action = action;

            isEnabled.ValueChanged += () => { CanExecuteChanged(this, new EventArgs()); };
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return isEnabled.Value;
        }

        public void Execute(object parameter)
        {
            action((T) parameter);
        }
    }
}
