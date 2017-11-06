using System;

namespace Reposify.Tests
{
    public class PolyType : TestsEntity
    {
        public enum Values
        {
            Val1 = 1,
            Val2 = 2,
            Val3 = 3,
        };

        protected PolyType() { }

        public virtual string       String              { get; protected set; }
        public virtual string       BigString           { get; protected set; }
        public virtual int          Int                 { get; protected set; }
        public virtual DateTime     DateTime            { get; protected set; }
        public virtual Values       Enum                { get; protected set; }
        public virtual int?         NullableInt         { get; protected set; }
        public virtual DateTime?    NullableDateTime    { get; protected set; }
        public virtual Values?      NullableEnum        { get; protected set; }
        public virtual PolyType     SubType             { get; protected set; }
    }
}
