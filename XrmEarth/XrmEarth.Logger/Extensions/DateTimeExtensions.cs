using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XrmEarth.Logger.Extensions
{
    public static class DateTimeExtensions
    {
        #region | Private Definitions |

        private static readonly DateTime EPOCH = DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0, 0), DateTimeKind.Utc);

        #endregion

        #region | Enums |

        public enum FrequencyType
        {
            None = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Quarterly = 4,
            Annually = 5
        }

        #endregion | Enums |

        #region | Public Methods |

        public static DateTime FromUnixTimestamp(long timestamp)
        {
            return EPOCH.AddSeconds(timestamp);
        }

        public static long ToUnixTimestamp(DateTime date)
        {
            TimeSpan diff = date.ToUniversalTime() - EPOCH;
            return (long)diff.TotalSeconds;
        }

        public static DateTime FromIso8601FormattedDateTime(string iso8601DateTime)
        {
            return DateTime.ParseExact(iso8601DateTime, "o", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToIso8601FormattedDateTime(DateTime dateTime)
        {
            return dateTime.ToString("o");
        }

        public static DateTime EquivalentWeekDay(this DateTime previousDate)
        {
            int num = (int)previousDate.DayOfWeek;
            int num2 = (int)DateTime.Today.DayOfWeek;
            return DateTime.Today.AddDays(num - num2);
        }

        public static List<DateTime> GetDaysInWeek(this DateTime date, DayOfWeek firstdayofweek)
        {
            List<DateTime> d = new List<DateTime>();

            int days = date.DayOfWeek - firstdayofweek;
            DateTime dt = date.AddDays(-days);
            d.Add(dt);
            d.AddRange(new DateTime[] { dt.AddDays(1), dt.AddDays(2), dt.AddDays(3), dt.AddDays(4), dt.AddDays(5), dt.AddDays(6) });

            return d;
        }

        public static IEnumerable<DateTime> Range(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }

        public static DateTime[] GetBeginAndEndDates(this DateTime date, FrequencyType frequency)
        {
            DateTime[] result = new DateTime[2];
            DateTime dateRangeBegin = date;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0); //One day
            DateTime dateRangeEnd = DateTime.Today.Add(duration);

            switch (frequency)
            {
                case FrequencyType.Daily:
                    dateRangeBegin = date;
                    dateRangeEnd = dateRangeBegin;
                    break;

                case FrequencyType.Weekly:
                    dateRangeBegin = date.AddDays(-(int)date.DayOfWeek);
                    dateRangeEnd = date.AddDays(6 - (int)date.DayOfWeek);
                    break;

                case FrequencyType.Monthly:
                    duration = new TimeSpan(DateTime.DaysInMonth(date.Year, date.Month) - 1, 0, 0, 0);
                    dateRangeBegin = date.AddDays((-1) * date.Day + 1);
                    dateRangeEnd = dateRangeBegin.Add(duration);
                    break;

                case FrequencyType.Quarterly:
                    int currentQuater = (date.Date.Month - 1) / 3 + 1;
                    int daysInLastMonthOfQuarter = DateTime.DaysInMonth(date.Year, 3 * currentQuater);
                    dateRangeBegin = new DateTime(date.Year, 3 * currentQuater - 2, 1);
                    dateRangeEnd = new DateTime(date.Year, 3 * currentQuater, daysInLastMonthOfQuarter);
                    break;

                case FrequencyType.Annually:
                    dateRangeBegin = new DateTime(date.Year, 1, 1);
                    dateRangeEnd = new DateTime(date.Year, 12, 31);
                    break;
            }

            result[0] = dateRangeBegin.Date;
            result[1] = dateRangeEnd.Date;

            return result;
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
        {
            int diff = date.DayOfWeek - startOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int GetWorkdaysOfMonth(this DateTime date)
        {
            int result = -1;

            List<DateTime> allDatesInMonth = new List<DateTime>();

            for (int i = 1; i < date.Day + 1; i++)
            {
                DateTime dt = new DateTime(date.Year, date.Month, i);

                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    allDatesInMonth.Add(dt);
                }
            }

            result = allDatesInMonth.Count;

            return result;
        }

        public static int Age(this DateTime dateOfBirth)
        {
            if (
                DateTime.Today.Month < dateOfBirth.Month ||
                DateTime.Today.Month == dateOfBirth.Month && DateTime.Today.Day < dateOfBirth.Day
                )
            {
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            }
            else
            {
                return DateTime.Today.Year - dateOfBirth.Year;
            }
        }

        public static bool IsWeekend(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday);
        }

        public static bool IsWeekday(this DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday: return false;

                default: return true;
            }
        }

        public static DateTime AddWorkdays(this DateTime d, int days)
        {
            // start from a weekday
            while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            for (int i = 0; i < days; ++i)
            {
                d = d.AddDays(1.0);
                while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            }
            return d;
        }

        public static TimeSpan Elapsed(this DateTime input)
        {
            return DateTime.Now.Subtract(input);
        }

        public static bool IsFuture(this DateTime date, DateTime from)
        {
            return date.Date > from.Date;
        }

        public static bool IsFuture(this DateTime date)
        {
            return date.IsFuture(DateTime.Now);
        }

        public static bool IsPast(this DateTime date, DateTime from)
        {
            return date.Date < from.Date;
        }

        public static bool IsPast(this DateTime date)
        {
            return date.IsPast(DateTime.Now);
        }

        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        #endregion
    }
}
