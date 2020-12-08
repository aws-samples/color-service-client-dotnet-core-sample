using System;
using System.Drawing;

namespace WebUIColorClient.Utility
{
    public static class Extensions
    {
        public static Exception Innermost(this Exception ex)
        {
            if (ex == null)
                throw new NullReferenceException(nameof(ex));

            while (ex.InnerException != null)
                ex = ex.InnerException;

            return ex;
        }

        public static string ToCssColor(this Color color) 
            => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        public static Color ToXorColor(this Color color)
        {
            uint intColor = (uint)color.ToArgb();
            uint xorWoAlpha = (intColor & 0x00FFFFFF) ^ 0xFFFFFFFF;
            uint alpha = intColor & 0xFF000000;
            uint xor = xorWoAlpha | alpha;
            return Color.FromArgb((int)xor);
        }
    }
}
