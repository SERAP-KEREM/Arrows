using UnityEngine;
using UnityEngine.UI;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace SerapKeremGameKit._UI
{
	public sealed class SettingsPanel : UIPanel
    {
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Toggle _hapticToggle;
        [SerializeField] private Button _closeButton;

        private const string SoundKey = _Utilities.PreferencesKeys.SettingsSound;
        private const string HapticKey = _Utilities.PreferencesKeys.SettingsHaptic;

        private void OnEnable()
        {
            if (_soundToggle != null) _soundToggle.isOn = PlayerPrefs.GetInt(SoundKey, 1) == 1;
            if (_hapticToggle != null) _hapticToggle.isOn = PlayerPrefs.GetInt(HapticKey, 1) == 1;
        }

		private void Awake()
		{
			if (_soundToggle != null) _soundToggle.onValueChanged.AddListener(OnSoundToggled);
			if (_hapticToggle != null) _hapticToggle.onValueChanged.AddListener(OnHapticToggled);
			if (_closeButton != null) _closeButton.BindOnClick(this, OnCloseClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_soundToggle != null) _soundToggle.onValueChanged.RemoveListener(OnSoundToggled);
			if (_hapticToggle != null) _hapticToggle.onValueChanged.RemoveListener(OnHapticToggled);
			// _closeButton auto-unsubscribed by ButtonExtensions
		}

        private void OnSoundToggled(bool value)
        {
            PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
            PlayerPrefs.Save();
            if (AudioManager.IsInitialized) AudioManager.Instance.SetEnabled(value);
        }

        private void OnHapticToggled(bool value)
        {
            PlayerPrefs.SetInt(HapticKey, value ? 1 : 0);
            PlayerPrefs.Save();
            if (HapticManager.IsInitialized) HapticManager.Instance.SetEnabled(value);
        }

        private void OnCloseClicked()
        {
            Hide();
        }
    }
}