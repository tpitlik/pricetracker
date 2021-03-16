using System;

namespace NlnrPriceDyn.Logic.Common
{
    public static class TimeHelper
    {
        public static long LastUpdateSecondsCount { get; private set; }

        static TimeHelper()
        {
            Update();
        }

        public static void Update()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            LastUpdateSecondsCount = dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}
