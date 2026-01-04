using SerapKeremGameKit._Pools;
using System.Collections.Generic;
using UnityEngine;

namespace SerapKeremGameKit._Audio
{
    public sealed class AudioPool : BasePool<AudioPlayer>
    {
        [SerializeField] private AudioPlayer _playerPrefab;

        protected override AudioPlayer Create()
        {
            if (_playerPrefab != null)
            {
                AudioPlayer instance = Instantiate(_playerPrefab, transform, false);
                instance.gameObject.SetActive(false);
                instance.ResetState();
                return instance;
            }

            GameObject go = new GameObject();
            go.name = nameof(AudioPlayer);
            go.transform.SetParent(transform, false);
            AudioPlayer created = go.AddComponent<AudioPlayer>();
            created.ResetState();
            return created;
        }

        protected override void OnGet(AudioPlayer item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnRecycle(AudioPlayer item)
        {
            item.ResetState();
            item.gameObject.SetActive(false);
        }

        protected override void OnStop(AudioPlayer item)
        {
            item.Stop();
        }
    }
}



