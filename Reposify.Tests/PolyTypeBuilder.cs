using System;
using Reposify.Testing;

namespace Reposify.Tests
{
    public class PolyTypeBuilder : Builder<PolyType>
    {
        public static readonly DateTime DefaultDateTimeValue = new DateTime(2001, 02, 03, 04, 05, 06);

        public PolyTypeBuilder(bool includeSubType = false)
        {
            With(u => u.String, "string value");
            With(u => u.BigString, new string('a', 9000));
            With(u => u.Int, 10);
            With(u => u.Boolean, true);
            With(u => u.DateTime, DefaultDateTimeValue);
            With(u => u.Enum, PolyType.Values.Val2);
            With(u => u.NullableString, "not null string value");
            With(u => u.NullableInt, 20);
            With(u => u.NullableBoolean, true);
            With(u => u.NullableEnum, PolyType.Values.Val3);
            With(u => u.NullableDateTime, DefaultDateTimeValue + TimeSpan.FromDays(3));

            if (includeSubType)
                With(u => u.SubType, new PolyTypeBuilder(false).Value());
        }
    }
}
