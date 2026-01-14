using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._UI;

namespace _Game.UI
{
    public class HeartPanel : MonoBehaviour
    {
        [Header("Heart References")]
        [SerializeField] private List<HeartUI> _hearts = new List<HeartUI>();

        private const int MaxHearts = 5;

        private void Awake()
        {
            if (_hearts.Count == 0)
            {
                _hearts.AddRange(GetComponentsInChildren<HeartUI>(true));
            }

            if (_hearts.Count != MaxHearts)
            {
                Debug.LogWarning($"{name}: Expected {MaxHearts} hearts, but found {_hearts.Count}. Please assign {MaxHearts} HeartUI components in Inspector.", this);
            }
        }

        public void Initialize()
        {
            foreach (var heart in _hearts)
            {
                if (heart != null)
                {
                    heart.Initialize();
                }
            }
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
