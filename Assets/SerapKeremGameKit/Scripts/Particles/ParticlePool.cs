using SerapKeremGameKit._Pools;
using UnityEngine;

namespace SerapKeremGameKit._Particles
{
    public sealed class ParticlePool : BasePool<ParticlePlayer>
    {
        [SerializeField] private ParticlePlayer _playerPrefab;

        protected override ParticlePlayer Create()
        {
            if (_playerPrefab != null)
            {
                ParticlePlayer instance = Instantiate(_playerPrefab, transform, false);
                instance.gameObject.SetActive(false);
                instance.ResetState();
                return instance;
            }

            GameObject go = new GameObject();
            go.name = nameof(ParticlePlayer);
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            ParticlePlayer created = go.AddComponent<ParticlePlayer>();
            created.ResetState();
            return created;
        }

        protected override void OnGet(ParticlePlayer item)
        {
            item.gameObject.SetActive(true); // stays idle until Play() is called by manager
        }

        protected override void OnRecycle(ParticlePlayer item)
        {
            item.ResetState();
            item.gameObject.SetActive(false);
        }

        protected override void OnStop(ParticlePlayer item)
        {
            item.Stop();
        }
    }
}


