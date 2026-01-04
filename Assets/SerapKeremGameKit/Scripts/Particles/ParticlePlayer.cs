using UnityEngine;

namespace SerapKeremGameKit._Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ParticlePlayer : MonoBehaviour
    {
        private ParticleSystem _ps;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
            if (_ps != null)
            {
                var main = _ps.main;
                main.playOnAwake = false; // do not auto-play on enable
            }
        }

        public void Play(ParticleSystem prefab, Vector3 position, Transform parent, float duration)
        {
            if (prefab == null) return;

            transform.SetParent(parent);
            if (parent != null)
            {
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.position = position;
            }

            CopyMain(prefab, _ps);
            var main = _ps.main;
            if (duration > 0f) main.duration = duration;
            _ps.Clear(true);
            _ps.Play(true);
        }

        public bool IsAlive()
        {
            return _ps != null && _ps.IsAlive(true);
        }

        public void Stop()
        {
            if (_ps != null)
            {
                _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void ResetState()
        {
            if (_ps == null)
            {
                _ps = GetComponent<ParticleSystem>();
            }
            if (_ps != null)
            {
                _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _ps.Clear(true);
                var main = _ps.main;
                main.playOnAwake = false;
            }
            transform.localScale = Vector3.one;
        }

        private static void CopyMain(ParticleSystem source, ParticleSystem target)
        {
            var s = source.main;
            var t = target.main;
            t.loop = s.loop;
            t.duration = s.duration;
            t.startLifetime = s.startLifetime;
            t.startSpeed = s.startSpeed;
            t.startSize = s.startSize;
            t.startRotation = s.startRotation;
            t.startColor = s.startColor;
            t.gravityModifier = s.gravityModifier;
            t.simulationSpace = s.simulationSpace;
            t.playOnAwake = false;
        }
    }
}


