using System;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Offsets to move the day of the year on a week, allowing
        ///     for the current year Jan 1st day of week, and the Sun/Mon
        ///     week start difference between ISO 8601 and Microsoft
        /// </summary>
        private static readonly int[] MoveByDays = {6, 7, 8, 9, 10, 4, 5};

        /// <summary>
        ///     Get the Week number of the year
        ///     (In the range 1..53)
        ///     This conforms to ISO 8601 specification for week number.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Week of year</returns>
        [UsedImplicitly]
        [Pure]
        public static int WeekOfYear(this DateTime date)
        {
            var startOfYear = new DateTime(date.Year, 1, 1);
            var endOfYear = new DateTime(date.Year, 12, 31);

            // ISO 8601 weeks start with Monday 
            // The first week of a year includes the first Thursday 
            // This means that Jan 1st could be in week 51, 52, or 53 of the previous year...
            var numberDays = date.Subtract(startOfYear).Days +
                             MoveByDays[(int) startOfYear.DayOfWeek];

            var weekNumber = numberDays / 7;
            if (weekNumber != 0)
            {
                if (weekNumber != 53)
                    return weekNumber;

                // In first week of next year.
                if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                    weekNumber = 1;
            }
            else
            {
                weekNumber = WeekOfYear(startOfYear.AddDays(-1));
            }

            return weekNumber;
        }
    }
}