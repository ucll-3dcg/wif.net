
namespace Cells
{    
    internal class ConcreteCell<T> : Cell<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialValue">
        /// Cell's initial value.
        /// </param>
        public ConcreteCell( T initialValue = default(T) )
            : base( initialValue )
        {
            // NOP
        }

        /// <summary>
        /// Value of the cell.
        /// </summary>
        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if ( !Util.AreEqual( base.Value, value ) )
                {
                    base.Value = value;
                    NotifyObservers();
                }
            }
        }

        public override string ToString()
        {
            return string.Format( "CELL[{0}]", this.Value != null ? this.Value.ToString() : "null" );
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
