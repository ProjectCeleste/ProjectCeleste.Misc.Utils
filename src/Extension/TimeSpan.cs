﻿using System;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class TimeSpanExtensions
    {
        public static string ToStringReadable(this TimeSpan span)
        {
            var formatted =
                (span.Duration().Days > 0
                    ? $"{span.Days:0} day{(span.Days == 1 ? string.Empty : "s")}, "
                    : string.Empty) +
                (span.Duration().Hours > 0
                    ? $"{span.Hours:0} hour{(span.Hours == 1 ? string.Empty : "s")}, "
                    : string.Empty) +
                (span.Duration().Minutes > 0
                    ? $"{span.Minutes:0} minute{(span.Minutes == 1 ? string.Empty : "s")}, "
                    : string.Empty) +
                (span.Duration().Seconds > 0
                    ? $"{span.Seconds:0} second{(span.Seconds == 1 ? string.Empty : "s")}"
                    : string.Empty);

            if (formatted.EndsWith(", "))
            {
                formatted = formatted.Substring(0, formatted.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(formatted))
            {
                formatted = "0 second";
            }

            return formatted;
        }
    }
}