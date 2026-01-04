using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Enums;

namespace SerapKeremGameKit._Data
{
    [CreateAssetMenu(menuName = "Data/ColorTypeToMaterialSO", fileName = "ColorTypeToMaterialSO", order = 0)]
    public class ColorTypeToMaterialSO : ScriptableObject
    {
        [System.Serializable]
        public class ColorTypeMaterialPair
        {
            [SerializeField] private ColorType _colorType;
            public ColorType ColorType => _colorType;

            [SerializeField] private Material _material;
            public Material Material => _material;
        }

        [ListDrawerSettings]
        [SerializeField] private List<ColorTypeMaterialPair> _colorMaterialPairs = new List<ColorTypeMaterialPair>();

        private readonly Dictionary<ColorType, Material> _lookup = new Dictionary<ColorType, Material>();

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
            for (int i = 0; i < _colorMaterialPairs.Count; i++)
            {
                ColorTypeMaterialPair pair = _colorMaterialPairs[i];
                _lookup[pair.ColorType] = pair.Material;
            }
        }

        public Material GetMaterial(ColorType colorType)
        {
            if (_lookup.TryGetValue(colorType, out Material mat) && mat != null)
            {
                return mat;
            }
            TraceLogger.LogError($"Material not found for ColorType: {colorType}");
            return null;
        }
    }
}