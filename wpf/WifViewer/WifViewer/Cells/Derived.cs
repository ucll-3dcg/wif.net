using System;

namespace Cells
{
    internal class Derived<T> : ConcreteCell<T>
    {
        private readonly Func<T> reader;

        private readonly Action<T> writer;

        public Derived( Func<T> reader, Action<T> writer )
            : base( reader() )
        {            
            this.reader = reader;
            this.writer = writer;
        }

        public Derived( Func<T> reader )
            : this( reader, _ => { throw new InvalidOperationException( "Cell is readonly" ); } )
        {
            // NOP
        }

        public override void Refresh()
        {
            base.Value = reader();
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                writer( value );
            }
        }
    }

}
