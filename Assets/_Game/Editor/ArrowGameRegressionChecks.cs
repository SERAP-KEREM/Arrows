using System;
using System.Reflection;
using _Game.Line;
using SerapKeremGameKit._InputSystem.Data;
using SerapKeremGameKit._UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Arrow = _Game.Line.Line;

namespace _Game.Editor
{
    public static class ArrowGameRegressionChecks
    {
        private static Scene _scene;

        [MenuItem("Tools/Arrow Game/Run Regression Checks")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Run the checks outside Play mode.");

            _scene = EditorSceneManager.NewPreviewScene();
            try
            {
                CheckInputCancellation();
                CheckHints();
                CheckAnimation(1f / 15f);
                CheckAnimation(1f / 30f);
                CheckAnimation(1f / 60f);
                CheckReverse();
                CheckHud();
                Debug.Log("Arrow Game regression checks passed.");
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(_scene);
            }
        }

        private static void CheckInputCancellation()
        {
            var input = ScriptableObject.CreateInstance<PlayerInputSO>();
            try
            {
                input.SetMouseDown(Vector3.one);
                Expect(input.Held && input.DownThisFrame, "Press was not recorded.");
                input.Cancel();
                Expect(!input.Held && !input.DownThisFrame && !input.UpThisFrame, "Canceled press remained active.");
                input.SetMouseUp(Vector3.zero);
                input.ResetFrame();
                Expect(!input.UpThisFrame, "Release repeated across frames.");
            }
            finally { Object.DestroyImmediate(input); }
        }

        private static void CheckHints()
        {
            GameObject root = Create("Hint fixtures");
            var manager = root.AddComponent<LineManager>();
            GameObject arrowObject = Create("Candidate", root.transform);
            var renderer = arrowObject.AddComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.positionCount = 2;
            renderer.SetPositions(new[] { Vector3.left, Vector3.zero });
            var animation = arrowObject.AddComponent<LineAnimation>();
            var arrow = arrowObject.AddComponent<Arrow>();
            GameObject headObject = Create("Head", arrowObject.transform);
            var head = headObject.AddComponent<LineRendererHead>();
            Set(arrow, "_lineRenderer", renderer);
            Set(arrow, "_animation", animation);
            Set(arrow, "_lineHead", head);
            arrow.Initialize(manager);

            Expect(manager.TryGetHint(out var hint) && hint == arrow, "An open arrow was not suggested.");

            GameObject blocker = Create("Blocker", root.transform);
            blocker.AddComponent<Arrow>();
            var collider = blocker.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.2f, 2f);
            collider.isTrigger = true;
            blocker.transform.position = new Vector3(2f, 0f, 0f);
            Expect(!manager.TryGetHint(out _), "Hint crossed a trigger blocker.");

            blocker.transform.position = new Vector3(-2f, 0f, 0f);
            Expect(manager.TryGetHint(out _), "A blocker behind the head prevented a hint.");
            Set(animation, "_isPlaying", true);
            Expect(!arrow.IsClickable, "A moving arrow remained clickable.");
            Expect(!manager.TryGetHint(out _), "A hint was offered during movement.");
            Set(animation, "_isPlaying", false);

            int completions = 0;
            manager.OnAllLinesRemoved += () => completions++;
            manager.UnregisterLine(arrow);
            manager.UnregisterLine(arrow);
            Expect(completions == 1, "Removing one arrow twice repeated the win event.");
            Object.DestroyImmediate(root);
        }

        private static void CheckAnimation(float step)
        {
            GameObject root = Create("Movement fixtures");
            LineAnimation animation = CreateAnimation(root, out LineRenderer renderer);
            int completions = 0;
            animation.OnAnimationCompleted += () => completions++;
            Set(animation, "_isPlaying", true);
            Set(animation, "_forward", true);

            for (int i = 0; i < 1000 && animation.IsPlaying; i++)
                Invoke(animation, "Advance", step);

            Expect(!animation.IsPlaying && completions == 1, $"Arrow did not exit at timestep {step}.");
            Expect(renderer.positionCount == 2, "Tail failed to consume the short bend.");
            Invoke(animation, "Advance", step);
            Expect(completions == 1, "Completion fired twice.");
            Object.DestroyImmediate(root);
        }

        private static void CheckReverse()
        {
            GameObject root = Create("Reverse fixtures");
            LineAnimation animation = CreateAnimation(root, out LineRenderer renderer);
            Set(animation, "_isPlaying", true);
            Set(animation, "_forward", true);
            Invoke(animation, "Advance", 0.2f);
            Set(animation, "_forward", false);
            for (int i = 0; i < 100 && animation.IsPlaying; i++) Invoke(animation, "Advance", 0.1f);
            Expect(!animation.IsPlaying && renderer.positionCount == 3, "Reverse did not restore the path.");
            Expect(Vector3.Distance(renderer.GetPosition(0), Vector3.zero) < 0.001f, "Reverse lost the original tail.");
            Expect(Vector3.Distance(renderer.GetPosition(2), new Vector3(3f, 0.05f, 0f)) < 0.001f,
                "Reverse lost the original head.");
            Object.DestroyImmediate(root);
        }

        private static LineAnimation CreateAnimation(GameObject root, out LineRenderer renderer)
        {
            var camera = Create("Camera", root.transform).AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 10f;
            camera.aspect = 1f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            renderer = Create("Arrow", root.transform).AddComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.positionCount = 3;
            renderer.SetPositions(new[] { Vector3.zero, new Vector3(0f, 0.05f, 0f), new Vector3(3f, 0.05f, 0f) });
            var animation = renderer.gameObject.AddComponent<LineAnimation>();
            animation.Initialize(renderer);
            Set(animation, "_gameCamera", camera);
            return animation;
        }

        private static void CheckHud()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SerapKeremGameKit/Resources/UI/InGameUI.prefab");
            Expect(prefab != null, "HUD prefab is missing.");
            var hud = new SerializedObject(prefab.GetComponent<HUDPanel>());
            var button = hud.FindProperty("_hintButton").objectReferenceValue as Button;
            Expect(button != null && button.image != null && button.image.sprite != null, "Hint button is not wired.");
            Expect(hud.FindProperty("_hintStatus").objectReferenceValue != null, "Hint status is not wired.");
        }

        private static GameObject Create(string name, Transform parent = null)
        {
            var gameObject = new GameObject(name);
            SceneManager.MoveGameObjectToScene(gameObject, _scene);
            if (parent != null) gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static void Set(object target, string field, object value)
        {
            target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        }

        private static void Invoke(object target, string method, float value)
        {
            target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, new object[] { value });
        }

        private static void Expect(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
