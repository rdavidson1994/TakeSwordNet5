namespace TakeSword
{
    public static class TimeUtilities
    {
        public static readonly int TicksPerSecond = 1;
        public static readonly int TicksPerMinute = TicksPerSecond * 60;
        public static readonly int TicksPerHour = TicksPerMinute * 60;
        public static readonly int TicksPerDay = TicksPerHour * 24;

        public static bool IsNight(int ticks)
        {
            return !IsDay(ticks);
        }

        public static bool IsDay(int ticks)
        {
            int hour = (ticks % TicksPerDay) / TicksPerHour;
            return (hour > 5 && hour < 12 + 7);
        }

        public static string ClockTime(int ticks)
        {
            int ticksSinceMidnight = (ticks % TicksPerDay);
            int hoursSinceMidnight = ticksSinceMidnight / TicksPerHour;
            int minutesSinceHour = ticksSinceMidnight - hoursSinceMidnight * TicksPerHour;
            string suffix;
            int displayedHour;
            if (hoursSinceMidnight > 12)
            {
                suffix = "pm";
                displayedHour = hoursSinceMidnight - 12;
            }
            else
            {
                suffix = "am";
                displayedHour = hoursSinceMidnight;
            }

            if (displayedHour == 0)
            {
                displayedHour = 12;
            }

            return $"{displayedHour}:{minutesSinceHour:D2} {suffix}";
        }

    }
}