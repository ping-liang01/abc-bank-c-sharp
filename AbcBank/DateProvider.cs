using System;

namespace AbcBank
{
    public class DateProvider
    {
        private static DateProvider _instance = null;

        public static DateProvider GetInstance()
        {
            return _instance ?? (_instance = new DateProvider());
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
