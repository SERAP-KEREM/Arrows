using UnityEngine;
using SerapKeremGameKit._Enums;

namespace SerapKeremGameKit._Color
{
    public static class ColorTypeToColor
    {
        public static Color GetColor(ColorType colorType)
        {
            return colorType switch
            {
                ColorType._0Empty => Color.clear,
                ColorType._1Green => new Color(0.18f, 0.80f, 0.44f), // Emerald
                ColorType._2Blue => new Color(0.20f, 0.60f, 0.86f), // Peter River
                ColorType._3Red => new Color(0.90f, 0.30f, 0.24f), // Pomegranate
                ColorType._4Yellow => new Color(1.00f, 0.80f, 0.20f), // Sunflower
                ColorType._5Purple => new Color(0.56f, 0.27f, 0.68f), // Wisteria
                ColorType._6Pink => new Color(1.00f, 0.40f, 0.60f), // Watermelon
                ColorType._7Orange => new Color(0.95f, 0.61f, 0.07f), // Orange
                ColorType._8Turquoise => new Color(0.10f, 0.74f, 0.61f), // Turquoise
                ColorType._9DarkBlue => new Color(0.17f, 0.24f, 0.31f), // Wet Asphalt
                ColorType._qBrown => new Color(0.55f, 0.27f, 0.07f), // Saddle Brown
                ColorType._wBlack => Color.black,
                ColorType._eNone => new Color(0f, 0f, 0f, 0f), // Fully transparent
                _ => Color.black // fallback
            };
        }
    }
}