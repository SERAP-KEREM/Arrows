using System;
using UnityEngine;
using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._Managers;

namespace _Game.UI
{
    public class LivesManager : MonoSingleton<LivesManager>
    {
        private const int MaxLives = 5;
        private int _currentLives;
        private int _lastLifeLossFrame = -1;

        public int CurrentLives => _currentLives;
        public int MaxLivesCount => MaxLives;

        public event Action<int> OnLivesChanged;
        public event Action OnLivesDepleted;

        protected override void Awake()
        {
            base.Awake();
            ResetLives();
        }

        public void ResetLives()
        {
            _currentLives = MaxLives;
            _lastLifeLossFrame = -1;
            OnLivesChanged?.Invoke(_currentLives);
        }

        public void LoseLife()
        {
            int currentFrame = Time.frameCount;
            
            if (_lastLifeLossFrame == currentFrame)
            {
                return;
            }

            if (_currentLives <= 0)
            {
                return;
            }

            _lastLifeLossFrame = currentFrame;
            _currentLives--;
            OnLivesChanged?.Invoke(_currentLives);

            if (_currentLives <= 0)
            {
                OnLivesDepleted?.Invoke();
            }
        }

        public bool HasLivesRemaining()
        {
            return _currentLives > 0;
        }
    }
}
