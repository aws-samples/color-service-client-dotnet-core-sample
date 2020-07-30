using SysColor = System.Drawing.Color;

namespace WebUIApp.Models
{
    public class Color
    {
        public string Value { get; set; } = "Green";

        private SysColor SystemColor => SysColor.FromName(this.Value);

        public string CssColor => ToCssColor(this.SystemColor);

        private static string ToCssColor(SysColor color) =>
            "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

        private static SysColor ToXorColor(SysColor color)
        {
            uint intColor = (uint)color.ToArgb();
            uint xorWoAlpha = (intColor & 0x00FFFFFF) ^ 0xFFFFFFFF;
            uint alpha = intColor & 0xFF000000;
            uint xor = xorWoAlpha | alpha;
            return SysColor.FromArgb((int)xor);
        }

        private SysColor XorColor => ToXorColor(this.SystemColor);

        public string CssXorColor => ToCssColor(this.XorColor);
    }
}