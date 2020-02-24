using System;
namespace CardUITest
{
    public class Helpers
    {
        public static double LimitToRange(double value, double inclusiveMinimum, double inlusiveMaximum)
        {
            if (value >= inclusiveMinimum)
            {
                return value <= inlusiveMaximum ? value : inlusiveMaximum;
            }

            return inclusiveMinimum;
        }

    }
}
