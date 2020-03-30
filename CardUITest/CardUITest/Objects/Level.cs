using System;
using System.Collections.Generic;
using System.Text;

namespace CardUITest.Objects
{
    class Level
    {

        public static double GetLevelFromXp(int xp)
        {
            double LevelMultiplier = 0.04;

            double level = LevelMultiplier * Math.Sqrt(xp);

            return Math.Floor(level);
        }
    }
}
