using System;
using UnityEngine;

namespace _Game.Line
{
    [RequireComponent(typeof(Collider2D))]
    public class LineHeadCollisionDetector : MonoBehaviour
    {
        public event Action<Collider2D> OnHeadCollision;
        
        private Line _ownLine;
        private bool _isInitialized;
        private bool _hasCollided = false;

        public void Initialize(Line ownLine)
        {
            _ownLine = ownLine;
            _isInitialized = true;
            _hasCollided = false;
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isInitialized || _ownLine == null || _hasCollided) return;
            CheckCollision(other);
        }

        private void CheckCollision(Collider2D other)
        {
            if (other == null || _hasCollided) return;

            Line otherLine = GetLineFromCollider(other);
            if (otherLine == null || otherLine == _ownLine)
            {
                return;
            }

            _hasCollided = true;
            OnHeadCollision?.Invoke(other);
        }

        private static Line GetLineFromCollider(Collider2D collider)
        {
            if (collider == null) return null;

            Line line = collider.GetComponent<Line>();
            if (line == null && collider.transform.parent != null)
            {
                line = collider.transform.parent.GetComponent<Line>();
            }
            return line;
        }

        public void ResetCollision()
        {
            _hasCollided = false;
        }
    }
}
