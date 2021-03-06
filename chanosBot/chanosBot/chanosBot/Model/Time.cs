using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model
{
    public struct Time
    {
        private const int MinHour = 0;
        private const int MaxHour = 23;

        private const int MinMinute = 0;
        private const int MaxMinute = 59;

        public int Hour { get; set; }

        public int Minute { get; set; }

        public static explicit operator TimeSpan(Time time)
        {
            return new TimeSpan(time.Hour, time.Minute, 0);
        }

        internal bool CheckOutOfTime()
        {
            if (Hour < MinHour ||
                Hour > MaxHour)
                return false;

            if (Minute < MinMinute ||
                Minute > MaxMinute)
                return false;

            return true;
        }

        internal static Time Empty()
        {
            return new Time()
            {
                Hour = -1,
                Minute = -1,
            };
        }

        internal static bool VerifyTime(string hour, string minute, out Time time)
        {
            time = Time.Empty();

            if (int.TryParse(hour, out int thour) &&
                int.TryParse(minute, out int tminute))
            {
                time.Hour = thour;
                time.Minute = tminute;
            }
            else
            {
                return false;
            }

            return time.CheckOutOfTime();
        }

        public override string ToString()
        {
            return $"Hour : {Hour}, Minute : {Minute}";
        }
    }
}
