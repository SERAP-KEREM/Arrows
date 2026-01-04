using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Singletons;
using UnityEditor;
using UnityEngine;
using Array2DEditor;
using TriInspector;
using System.Reflection;
using System;

namespace SerapKeremGameKit._Tile
{
    public sealed class TileManager : MonoSingleton<TileManager>
    {
        [Header("Grid Settings")]
        [SerializeField] private GameObject _parentObject;
        [SerializeField, Range(0.1f, 100f)] private float _distance = 1f;
        [Title("Grid Settings"), PropertyOrder(2)]
        [SerializeField] private Array2DInt _tileSizeArray; // Drives grid size/content via Array2DEditor

        [SerializeField, HideInInspector] private int _lastWidth;
        [SerializeField, HideInInspector] private int _lastHeight;

        // Prefab is taken from TileSpawner; no need to duplicate here

#if UNITY_EDITOR
        [Button("Build Grid (Editor)")]
        [ContextMenu("Editor/Build Grid")]
        [DisableInPlayMode]
        private void EditorBuildGrid()
        {
            if (Application.isPlaying) return;
            // Find spawner also in edit mode (Awake not called yet)
            TileSpawner spawner = TileSpawner.IsInitialized
                ? TileSpawner.Instance
                : UnityEngine.Object.FindFirstObjectByType<TileSpawner>(FindObjectsInactive.Include);
            if (spawner == null)
            {
                TraceLogger.LogError("TileSpawner is missing in the scene.", this);
                return;
            }

            if (_tileSizeArray == null)
            {
                TraceLogger.LogError("Array2DInt reference is missing.", this);
                return;
            }

            Transform parent = _parentObject != null ? _parentObject.transform : transform;

            ClearEditor();

            var cells = _tileSizeArray.GetCells(); // int[,]
            // Array2DEditor is typically row-major: [row(y), column(x)]
            int height = cells.GetLength(0);
            int width = cells.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int v = cells[y, x];
                    if (v <= 0) continue; // 0 => empty, >0 => place tile

                    float worldY = (height - 1 - y) * _distance; // y=0 at top
                    Vector3 pos = new Vector3(x * _distance, worldY, 0f) + parent.position;
                    Tile tile = spawner.SpawnTileInEditor(pos, parent);
                    if (tile != null)
                    {
                        tile.Initialize(new Vector2Int(x, y));
                        tile.name = $"Tile [{x},{y}]";
                        Undo.RegisterCreatedObjectUndo(tile.gameObject, "Create Tile");
                        EditorUtility.SetDirty(tile);
                    }
                    else
                    {
                        TraceLogger.LogError("Failed to spawn tile at (" + x + "," + y + ") worldPos=" + pos, this);
                    }
                }
            }
        }

        [Button("Clear Grid (Editor)")]
        [ContextMenu("Editor/Clear Grid")]
        [DisableInPlayMode]
        private void ClearEditor()
        {
            if (Application.isPlaying) return;
            Transform root = _parentObject == null ? transform : _parentObject.transform;
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                GameObject go = root.GetChild(i).gameObject;
                Undo.DestroyObjectImmediate(go);
            }
            EditorUtility.SetDirty(gameObject);
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            if (_tileSizeArray == null) return;

            // Ensure only newly added cells default to 1 in the inspector grid
            var cells = _tileSizeArray.GetCells();
            int height = cells.GetLength(0);
            int width = cells.GetLength(1);
            int prevW = Math.Max(0, _lastWidth);
            int prevH = Math.Max(0, _lastHeight);
            if (width > prevW || height > prevH)
            {
                // Write 1 only into the newly added columns/rows
                var setCell = _tileSizeArray.GetType().GetMethod(
                    "SetCell",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(int), typeof(int), typeof(int) },
                    null);

                if (setCell != null)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            bool isNewCell = (x >= prevW) || (y >= prevH);
                            if (!isNewCell) continue;
                            if (cells[y, x] == 0)
                                setCell.Invoke(_tileSizeArray, new object[] { x, y, 1 });
                        }
                    }
                    EditorUtility.SetDirty(this);
                }
                else
                {
                    // Fallback: SetCells if available
                    var setCells = _tileSizeArray.GetType().GetMethod(
                        "SetCells",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new System.Type[] { typeof(int[,]) },
                        null);
                    if (setCells != null)
                    {
                        var filled = (int[,])cells.Clone();
                        for (int y = 0; y < height; y++)
                            for (int x = 0; x < width; x++)
                                if ((x >= prevW || y >= prevH) && filled[y, x] == 0) filled[y, x] = 1;
                        setCells.Invoke(_tileSizeArray, new object[] { filled });
                        EditorUtility.SetDirty(this);
                    }
                }
            }

            _lastWidth = width;
            _lastHeight = height;
#endif
        }
#endif
    }
}


