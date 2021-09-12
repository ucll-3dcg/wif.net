using System;
using System.Diagnostics;

namespace Cells
{
    [DebuggerDisplay( "{Value}" )]
    public class Var<T> : IVar<T>
    {
        private T value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialValue">
        /// Var's initial value.
        /// </param>
        public Var( T initialValue = default(T) )
        {
            this.value = initialValue;
        }

        /// <summary>
        /// Value of the Var.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            return string.Format( "VAR[{0}]", value != null ? value.ToString() : "null" );
        }

        public override bool Equals( object obj )
        {
            throw new NotImplementedException( "Equals is not implemented for Vars" );
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException( "GetHashCode is not implemented for Vars" );
        }
    }
}
