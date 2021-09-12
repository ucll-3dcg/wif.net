using System;
using System.Windows.Input;
using Cells;

namespace Commands
{
    public abstract class CellCommand : ICommand
    {
        private readonly Cell<bool> isEnabled;

        public static CellCommand FromDelegate(Cell<bool> isEnabled, Action action)
        {
            return new DelegateCellCommand<object>( isEnabled, _ => action() );
        }

        protected CellCommand(Cell<bool> isEnabled)
        {
            if ( isEnabled == null )
            {
                throw new ArgumentNullException( "isEnabled" );
            }
            else
            {
                this.isEnabled = isEnabled;

                isEnabled.ValueChanged += () =>
                {
                    if ( CanExecuteChanged != null )
                    {
                        CanExecuteChanged( this, new EventArgs() );
                    }
                };
            }
        }

        public bool CanExecute( object parameter )
        {
            return isEnabled.Value;
        }

        public event EventHandler CanExecuteChanged;

        public abstract void Execute( object parameter );

        private class DelegateCellCommand<T> : CellCommand
        {
            private readonly Action<T> action;

            public DelegateCellCommand(Cell<bool> isEnabled, Action<T> action)
                : base(isEnabled)
            {
                this.action = action;
            }

            public override void Execute( object parameter )
            {
                action( (T) parameter );
            }
        }
    }
}
