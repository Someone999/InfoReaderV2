using System.Collections.Generic;
using System.Drawing;

namespace InfoReader.Tools
{
    public class FontInfo:IEqualityComparer<FontInfo>
    {
        public string FontName { get; set; } = "";
        public float FontSize { get; set; }


        public bool Equals(FontInfo x, FontInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x.GetType() != y.GetType()) return false;
            return x.FontName == y.FontName && x.FontSize.Equals(y.FontSize);
        }

        public int GetHashCode(FontInfo obj)
        {
            unchecked
            {
                return (obj.FontName.GetHashCode() * 397) ^ obj.FontSize.GetHashCode();
            }
        }
    }
    public static class FontTools
    {
        public static Font MicrosoftYaHei { get; } = new(new FontFamily("微软雅黑"), 12);
        private static readonly Dictionary<FontInfo, Font> Fonts = new Dictionary<FontInfo, Font>();

        public static Font GetFont(string fontName, float fontSize)
        {
            FontInfo info = new FontInfo
            {
                FontName = fontName,
                FontSize = fontSize
            };
            if (!Fonts.ContainsKey(info))
            {
                Fonts.Add(info, new Font(new FontFamily(fontName), fontSize));
            }
            return Fonts[info];
        }
    }
}
