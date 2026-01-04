using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using SerapKeremGameKit._Enums;
using SerapKeremGameKit._Logging;

namespace SerapKeremGameKit._Data
{
    [CreateAssetMenu(menuName = "Data/ColorTypeToColorSO", fileName = "ColorTypeToColorSO", order = 0)]
    public class ColorTypeToColorSO : ScriptableObject
    {
        [System.Serializable]
        public class ColorTypeColorPair
        {
            [SerializeField] private ColorType _colorType;
            public ColorType ColorType => _colorType;

            [SerializeField] private Color _color = Color.white;
            public Color Color => _color;
        }

        [ListDrawerSettings]
        [SerializeField] private List<ColorTypeColorPair> _colorPairs = new List<ColorTypeColorPair>();

        private readonly Dictionary<ColorType, Color> _lookup = new Dictionary<ColorType, Color>();

        private void OnEnable()
        {
            RebuildLookup();
        }

        private void OnValidate()
        {
            RebuildLookup();
        }

        private void RebuildLookup()
        {
            _lookup.Clear();
            for (int i = 0; i < _colorPairs.Count; i++)
            {
                ColorTypeColorPair pair = _colorPairs[i];
                _lookup[pair.ColorType] = pair.Color;
            }
        }

        public Color GetColor(ColorType colorType)
        {
            if (_lookup.TryGetValue(colorType, out Color color))
            {
                return color;
            }
            TraceLogger.LogError($"Color not found for ColorType: {colorType}");
            return Color.black;
        }
    }
}


