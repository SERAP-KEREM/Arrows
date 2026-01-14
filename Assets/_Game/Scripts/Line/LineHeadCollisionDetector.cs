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

            Line otherLine = other.GetComponent<Line>();
            if (otherLine == null && other.transform.parent != null)
            {
                otherLine = other.transform.parent.GetComponent<Line>();
            }

            if (otherLine == null || otherLine == _ownLine)
            {
                return;
            }

            _hasCollided = true;
            OnHeadCollision?.Invoke(other);
        }

        public void ResetCollision()
        {
            _hasCollided = false;
        }
    }
}
