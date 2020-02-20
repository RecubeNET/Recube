using System;

namespace Recube.Api.Util
{
    public static class MathHelper
    {
        public static float Clamp(float min, float max, float value)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}