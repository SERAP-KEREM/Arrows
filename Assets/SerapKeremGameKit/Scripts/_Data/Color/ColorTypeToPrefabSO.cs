using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using SerapKeremGameKit._Enums;
using SerapKeremGameKit._Logging;

namespace SerapKeremGameKit._Data
{
    [CreateAssetMenu(menuName = "Data/ColorTypeToPrefabSO", fileName = "ColorTypeToPrefabSO", order = 0)]
    public class ColorTypeToPrefabSO : ScriptableObject
    {
        [System.Serializable]
        public class ColorTypePrefabPair
        {
            // Removed EnumToggleButtons (requires Pro version)
            [SerializeField] private ColorType _colorType;
            public ColorType ColorType { get => _colorType; }

            // Removed PreviewField (requires Pro version)
            [SerializeField] private GameObject _prefab;
            public GameObject Prefab { get => _prefab; }
        }

        // Simplified ListDrawerSettings (removed Pro features)
        [ListDrawerSettings]
        [SerializeField] private List<ColorTypePrefabPair> _colorPrefabPairs = new List<ColorTypePrefabPair>();

        private readonly Dictionary<ColorType, GameObject> _lookup = new Dictionary<ColorType, GameObject>();

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
            for (int i = 0; i < _colorPrefabPairs.Count; i++)
            {
                ColorTypePrefabPair pair = _colorPrefabPairs[i];
                _lookup[pair.ColorType] = pair.Prefab;
            }
        }

        public GameObject GetPrefab(ColorType colorType)
        {
            if (_lookup.TryGetValue(colorType, out GameObject prefab) && prefab != null)
            {
                return prefab;
            }
            TraceLogger.LogError($"Prefab not found for ColorType: {colorType}");
            return null;
        }
    }
}