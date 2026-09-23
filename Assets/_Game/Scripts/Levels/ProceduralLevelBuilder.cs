using System;
using ArrowNook.Puzzles;
using SerapKeremGameKit._LevelSystem;
using UnityEngine;
using Arrow = _Game.Line.Line;

namespace ArrowNook.Levels
{
    public static class ProceduralLevelBuilder
    {
        public static void Populate(Level level, PuzzleDefinition puzzle)
        {
            if (level.LinesParent == null || level.LineManager == null)
                throw new InvalidOperationException("Generated level template has missing line references.");
            Arrow prefab = Resources.Load<Arrow>("Line/Line (1)");
            if (prefab == null) throw new InvalidOperationException("Arrow prefab is missing.");

            foreach (Arrow oldArrow in level.GetComponentsInChildren<Arrow>(true))
            {
                oldArrow.gameObject.SetActive(false);
                oldArrow.transform.SetParent(null);
                UnityEngine.Object.Destroy(oldArrow.gameObject);
            }

            const float spacing = 1.25f;
            foreach (ArrowPath path in puzzle.Arrows)
            {
                Arrow arrow = UnityEngine.Object.Instantiate(prefab, level.LinesParent);
                arrow.transform.localPosition = Vector3.zero;
                arrow.transform.localRotation = Quaternion.identity;
                arrow.transform.localScale = Vector3.one;
                var points = new Vector3[path.Cells.Count];
                for (int i = 0; i < points.Length; i++)
                    points[i] = new Vector3((path.Cells[i].X - (puzzle.Width - 1) * 0.5f) * spacing,
                        (path.Cells[i].Y - (puzzle.Height - 1) * 0.5f) * spacing, 0f);
                arrow.LineRenderer.useWorldSpace = false;
                arrow.LineRenderer.positionCount = points.Length;
                arrow.LineRenderer.SetPositions(points);
            }
        }
    }
}
