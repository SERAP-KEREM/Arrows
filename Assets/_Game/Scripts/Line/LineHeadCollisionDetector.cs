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

        public void Initialize(Line ownLine)
        {
            _ownLine = ownLine;
            _isInitialized = true;
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isInitialized || _ownLine == null) return;
            CheckCollision(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_isInitialized || _ownLine == null) return;
            CheckCollision(collision.collider);
        }

        private void CheckCollision(Collider2D other)
        {
            if (other == null) return;

            Line otherLine = other.GetComponent<Line>();
            if (otherLine == null && other.transform.parent != null)
            {
                otherLine = other.transform.parent.GetComponent<Line>();
            }

            if (otherLine == null || otherLine == _ownLine)
            {
                return;
            }

            OnHeadCollision?.Invoke(other);
        }
    }
}
