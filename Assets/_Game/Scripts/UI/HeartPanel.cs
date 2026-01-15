using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._UI;

namespace _Game.UI
{
    public class HeartPanel : MonoBehaviour
    {
        [Header("Heart References")]
        [SerializeField] private List<HeartUI> _hearts = new List<HeartUI>();

        private bool _isInitialized = false;

        private int MaxHearts
        {
            get
            {
                if (LivesManager.IsInitialized && LivesManager.Instance != null)
                {
                    return LivesManager.Instance.MaxLivesCount;
                }
                return 5; // Fallback default
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            int expectedHearts = MaxHearts;
            if (_hearts.Count != expectedHearts)
            {
                Debug.LogWarning($"{name}: Expected {expectedHearts} hearts, but found {_hearts.Count}. Please assign {expectedHearts} HeartUI components in Inspector.", this);
            }

            foreach (var heart in _hearts)
            {
                if (heart != null)
                {
                    heart.Initialize();
                }
            }

            _isInitialized = true;
        }

        public void UpdateHearts(int activeLives)
        {
            for (int i = 0; i < _hearts.Count; i++)
            {
                if (_hearts[i] != null)
                {
                    bool isActive = i < activeLives;
                    _hearts[i].SetActive(isActive);
                }
            }
        }

        public void ResetHearts()
        {
            UpdateHearts(MaxHearts);
        }
    }
}
