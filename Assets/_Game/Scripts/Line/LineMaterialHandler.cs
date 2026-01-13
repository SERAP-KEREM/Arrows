using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Line
{
    public class LineMaterialHandler : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private List<Component> _renderers = new List<Component>();

        [Header("Color Settings")]
        [SerializeField] private Color _failureColor = Color.red;
        [SerializeField] private float _failureColorDuration = 0.5f;

        private Dictionary<Component, Color> _originalColors = new Dictionary<Component, Color>();
        private Coroutine _colorResetCoroutine;

        private void Awake()
        {
            StoreOriginalColors();
        }

        private void StoreOriginalColors()
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    StoreRendererColor(renderer);
                }
            }
        }

        private void StoreRendererColor(Component renderer)
        {
            if (renderer is LineRenderer lineRenderer)
            {
                if (lineRenderer.sharedMaterial != null)
                {
                    lineRenderer.material = new Material(lineRenderer.sharedMaterial);
                }
                
                if (lineRenderer.material != null)
                {
                    _originalColors[renderer] = lineRenderer.material.color;
                }
            }
            else if (renderer is SpriteRenderer spriteRenderer)
            {
                _originalColors[renderer] = spriteRenderer.color;
            }
        }

        public void SetFailureColor()
        {
            SetColor(_failureColor);

            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
            }

            _colorResetCoroutine = StartCoroutine(ResetColorAfterDelay());
        }

        private IEnumerator ResetColorAfterDelay()
        {
            yield return new WaitForSeconds(_failureColorDuration);
            ResetToOriginalColors();
            _colorResetCoroutine = null;
        }

        public void ResetToOriginalColors()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }

            foreach (var kvp in _originalColors)
            {
                if (kvp.Key == null) continue;

                if (kvp.Key is LineRenderer lineRenderer && lineRenderer.material != null)
                {
                    lineRenderer.material.color = kvp.Value;
                }
                else if (kvp.Key is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = kvp.Value;
                }
            }
        }

        private void OnDestroy()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }
        }

        private void SetColor(Color color)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;

                if (renderer is LineRenderer lineRenderer)
                {
                    if (lineRenderer.material != null)
                    {
                        lineRenderer.material.color = color;
                    }
                }
                else if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = color;
                }
            }
        }

        public void AddRenderer(Component renderer)
        {
            if (renderer != null && !_renderers.Contains(renderer))
            {
                _renderers.Add(renderer);
                StoreRendererColor(renderer);
            }
        }
    }
}
