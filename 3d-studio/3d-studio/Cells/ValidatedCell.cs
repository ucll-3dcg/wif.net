using System;

namespace Cells
{
    internal class ValidatedCell<T> : ConcreteCell<T>
    {
        private readonly Func<T, bool> validator;

        public ValidatedCell(T initialValue, Func<T, bool> validator)
            : base(initialValue)
        {
            this.validator = validator;
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if ( !validator( value ) )
                {
                    throw new ArgumentException( "Invalid value" );
                }
                else
                {
                    base.Value = value;
                }
            }
        }
    }
}
