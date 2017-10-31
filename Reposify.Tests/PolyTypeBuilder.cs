using System;
using Reposify.Testing;

namespace Reposify.Tests
{
    public class PolyTypeBuilder : Builder<PolyType>
    {
        public static readonly DateTime DefaultDateTimeValue = new DateTime(2001, 02, 03, 04, 05, 06);

        public PolyTypeBuilder()
        {
            With(u => u.String, "string value");
            With(u => u.BigString, new string('a', 9000));
            With(u => u.Int, 10);
            With(u => u.DateTime, DefaultDateTimeValue);
            With(u => u.Enum, PolyType.Values.Val2);
        }
    }
}
