using SerapKeremGameKit._Singletons;
using UnityEditor;
using UnityEngine;

namespace SerapKeremGameKit._Tile
{
    public class TileSpawner : MonoSingleton<TileSpawner>
    {
        [SerializeField]
        private Tile _tilePrefab;

        public Tile SpawnTile(Vector3 position, Transform parentTransform)
        {
            Tile tile = Instantiate(_tilePrefab, position, Quaternion.identity, parentTransform);
            return tile;
        }

        public Tile SpawnTileInEditor(Vector3 position, Transform parentTransform)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject tileGO = PrefabUtility.InstantiatePrefab(_tilePrefab.gameObject, parentTransform) as GameObject;
                tileGO.transform.position = position;
                tileGO.transform.rotation = Quaternion.identity;

                Tile tile = tileGO.GetComponent<Tile>();
                return tile;
            }
#endif
            return null;
        }
    }
}


