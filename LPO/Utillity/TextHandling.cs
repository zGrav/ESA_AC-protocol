using System;

namespace LPO.Utillity
{
    class TextHandling // Class responsible for parsing text
    {
        internal static int Parse(string input)
        {
            int sum = 0, i = 0, length = input.Length, k;

            while (i < length)
            {
                k = input[i++];
                if (k < 48 || k > 59)
                    return 0;
                sum = 10 * sum + k - 48;
            }

            return sum;
        }

        internal static int GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double unixTime = ts.TotalSeconds;
            return (int)unixTime;
        }
    }
}
