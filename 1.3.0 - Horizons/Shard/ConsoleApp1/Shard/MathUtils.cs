using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    public static class MathUtils
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Math.Clamp(t, 0f, 1f);
        }

        public static bool Approximately(float a, float b, float tolerance = 0.0001f)
        {
            return Math.Abs(a - b) < tolerance;
        }
    }
}
