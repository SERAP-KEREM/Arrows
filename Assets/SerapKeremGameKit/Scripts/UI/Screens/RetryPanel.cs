using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class RetryPanel : UIPanel
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private UIRootController _uiRoot;

		private void Awake()
		{
			if (_yesButton != null) _yesButton.BindOnClick(this, OnYes);
			if (_noButton != null) _noButton.BindOnClick(this, OnNo);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			// Auto-unsubscribe handled by ButtonExtensions
		}

        private void OnYes()
        {
			if (_uiRoot != null) _uiRoot.OnRestartConfirmed();
        }

        private void OnNo()
        {
            Hide();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}