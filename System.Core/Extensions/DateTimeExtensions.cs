using System;

<<<<<<< HEAD:Connector/Extensions/DateTimeExtensions.cs
namespace Connector.Extensions
=======
namespace Bitmex.Core.Extensions
>>>>>>> * Bitmex.Runner.csproj: Projects ported to netcore:System.Core/Extensions/DateTimeExtensions.cs
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            var epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime-epochTime).TotalSeconds;
        }
    }
}
