using System;
using UnityEngine;
using UnityEngine.UI;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace SerapKeremGameKit._UI
{
	internal sealed class ButtonClickBinding : MonoBehaviour
	{
		public Button Button;
		public UnityEngine.Events.UnityAction Handler;

		private void OnDestroy()
		{
			if (Button != null && Handler != null)
			{
				Button.onClick.RemoveListener(Handler);
			}
		}
	}

	public static class ButtonExtensions
	{
		public static void BindOnClick(this Button button, MonoBehaviour owner, Action action)
		{
			if (button == null || owner == null || action == null) return;
			UnityEngine.Events.UnityAction handler = () => action();
			button.onClick.AddListener(handler);
			var binding = owner.gameObject.AddComponent<ButtonClickBinding>();
			binding.Button = button;
			binding.Handler = handler;
		}
	}
}