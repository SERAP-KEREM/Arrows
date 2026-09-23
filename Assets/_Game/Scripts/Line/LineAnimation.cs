using System;
using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace _Game.Line
{
    public class LineAnimation : MonoBehaviour
    {
        private LineRenderer line;
        [SerializeField] private float speed = 5f;
        [SerializeField] private string _movementSoundKey = "";

        private bool _isPlaying;
        private bool _forward;
        private Vector3 _direction;
        private Vector3[] positionsOrigin;
        private bool _isInitialized;
        private Vector3[] _tempPositionsArray;
        private Vector3ArrayPool _arrayPool;
        private float _visualZOffset;
        private Camera _gameCamera;
        private Line _ownLine;
        private readonly List<RaycastHit2D> _sweepHits = new List<RaycastHit2D>();

        public bool IsPlaying => _isPlaying;
        public bool IsForward => _forward;
        public Vector3 Direction => _direction;
        public float VisualZOffset
        {
            get => _visualZOffset;
            set => _visualZOffset = value;
        }

        public event Action<bool> OnAnimationStarted;
        public event Action OnAnimationStopped;
        public event Action OnAnimationCompleted;
        public event Action OnLinePositionsChanged;
        public event Action<Collider2D> OnForwardCollision;

        public void Initialize(LineRenderer lineRenderer, Vector3ArrayPool arrayPool = null)
        {
            if (lineRenderer == null) return;

            line = lineRenderer;
            _ownLine = GetComponent<Line>();
            _gameCamera = Camera.main;

            var count = line.positionCount;
            if (count < 2) return;

            positionsOrigin = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                positionsOrigin[i] = line.GetPosition(i);
            }

            var lastPoint = line.GetPosition(count - 1);
            _direction = lastPoint - line.GetPosition(count - 2);

            _arrayPool = arrayPool;

            _isInitialized = true;
            enabled = false;
        }

        public void Play(bool forwardDirection)
        {
            if (!_isInitialized || line == null || line.positionCount < 2)
                return;

            bool wasPlaying = _isPlaying;
            _forward = forwardDirection;
            _isPlaying = true;
            enabled = true;

            if (!wasPlaying)
            {
                OnAnimationStarted?.Invoke(forwardDirection);

                if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_movementSoundKey))
                    AudioManager.Instance.Play(_movementSoundKey);
                if (HapticManager.IsInitialized)
                    HapticManager.Instance.Play(HapticType.Selection);
            }
        }


        public void Stop()
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                enabled = false;
                OnAnimationStopped?.Invoke();
            }

            if (_tempPositionsArray != null && _arrayPool != null)
            {
                _arrayPool.RecycleArray(_tempPositionsArray);
                _tempPositionsArray = null;
            }
        }

        private void OnDestroy()
        {
            if (_tempPositionsArray != null && _arrayPool != null)
            {
                _arrayPool.RecycleArray(_tempPositionsArray);
                _tempPositionsArray = null;
            }
        }

        private void Update()
        {
            Advance(Time.deltaTime);
        }

        private void Advance(float deltaTime)
        {
            if (!_isPlaying) return;
            if (!line || line.positionCount < 2)
            {
                _isPlaying = false;
                enabled = false;
                OnAnimationStopped?.Invoke();
                return;
            }

            if (_forward)
                AnimateForward(deltaTime);
            else
                AnimateBackward(deltaTime);

            ApplyVisualZOffset();
        }

        private void ApplyVisualZOffset()
        {
            if (Mathf.Abs(_visualZOffset) < 0.001f) return;

            for (int i = 0; i < line.positionCount; i++)
            {
                Vector3 pos = line.GetPosition(i);
                pos.z = _visualZOffset;
                line.SetPosition(i, pos);
            }
        }

        private void AnimateForward(float deltaTime)
        {
            var count = line.positionCount;
            var lastPoint = line.GetPosition(count - 1);

            float travel = Mathf.Max(0f, speed * deltaTime);
            // Clamp travel at bends so low frame rates cannot skip past a tail waypoint.
            if (count > 2)
                travel = Mathf.Min(travel, Vector2.Distance(line.GetPosition(0), line.GetPosition(1)));
            Collider2D collision = FindForwardCollision(travel, out travel);
            lastPoint += _direction.normalized * travel;
            line.SetPosition(count - 1, lastPoint);

            var tailPoint = line.GetPosition(0);
            tailPoint = Vector3.MoveTowards(tailPoint, line.GetPosition(1), travel);
            line.SetPosition(0, tailPoint);

            OnLinePositionsChanged?.Invoke();

            if (collision != null)
            {
                OnForwardCollision?.Invoke(collision);
                return;
            }

            if (IsOutsideViewport())
            {
                _isPlaying = false;
                enabled = false;
                OnAnimationCompleted?.Invoke();
                return;
            }

            if (!(Vector2.Distance(tailPoint, line.GetPosition(1)) < 0.1f)) return;


            var newCount = count - 1;
            if (_arrayPool != null)
            {
                _tempPositionsArray = _arrayPool.GetArray(newCount);
            }
            else
            {
                _tempPositionsArray = new Vector3[newCount];
            }

            for (int i = 1; i < count; i++)
            {
                _tempPositionsArray[i - 1] = line.GetPosition(i);
            }

            line.positionCount = newCount;
            line.SetPositions(_tempPositionsArray);

            if (_arrayPool != null)
            {
                _arrayPool.RecycleArray(_tempPositionsArray);
            }
            _tempPositionsArray = null;

            OnLinePositionsChanged?.Invoke();

            if (newCount < 2)
            {
                _isPlaying = false;
                enabled = false;
                OnAnimationCompleted?.Invoke();
            }
        }

        private Collider2D FindForwardCollision(float travel, out float allowedTravel)
        {
            allowedTravel = travel;
            CircleCollider2D head = _ownLine != null ? _ownLine.HeadCollider : null;
            if (head == null || travel <= 0) return null;
            Vector3 origin = line.GetPosition(line.positionCount - 1);
            Vector3 movement = _direction.normalized * travel;
            if (!line.useWorldSpace)
            {
                origin = line.transform.TransformPoint(origin);
                movement = line.transform.TransformVector(movement);
            }
            origin += head.transform.TransformVector(head.offset);
            float distance = ((Vector2)movement).magnitude;
            if (distance < 0.0001f) return null;
            float radius = head.radius * Mathf.Max(Mathf.Abs(head.transform.lossyScale.x), Mathf.Abs(head.transform.lossyScale.y));
            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useTriggers = true;
            Physics2D.SyncTransforms();
            _sweepHits.Clear();
            gameObject.scene.GetPhysicsScene2D().CircleCast(origin, radius, movement.normalized, distance, filter, _sweepHits);
            Collider2D closest = null;
            float closestDistance = distance;
            foreach (RaycastHit2D hit in _sweepHits)
            {
                Line other = hit.collider != null ? hit.collider.GetComponentInParent<Line>() : null;
                if (other == null || other == _ownLine || hit.distance > closestDistance) continue;
                closest = hit.collider;
                closestDistance = hit.distance;
            }
            if (closest != null) allowedTravel = travel * Mathf.Max(0, closestDistance - 0.005f) / distance;
            return closest;
        }

        private bool IsOutsideViewport()
        {
            if (_gameCamera == null) return false;
            bool left = true, right = true, above = true, below = true;
            for (int i = 0; i < line.positionCount; i++)
            {
                Vector3 point = line.GetPosition(i);
                if (!line.useWorldSpace) point = line.transform.TransformPoint(point);
                Vector3 viewport = _gameCamera.WorldToViewportPoint(point);
                left &= viewport.x < -0.1f;
                right &= viewport.x > 1.1f;
                above &= viewport.y > 1.1f;
                below &= viewport.y < -0.1f;
            }
            return left || right || above || below;
        }

        private void AnimateBackward(float deltaTime)
        {
            int lastIndex = line.positionCount - 1;
            Vector3 currentHeadPos = line.GetPosition(lastIndex);
            Vector3 headMoveDir = -_direction.normalized;
            Vector3 originHeadPos = positionsOrigin[positionsOrigin.Length - 1];

            float distToOrigin = Vector2.Distance(currentHeadPos, originHeadPos);
            float moveDist = Mathf.Max(0f, speed * deltaTime);

            Vector3 newHeadPos;

            if (distToOrigin > 0.001f)
            {
                Vector3 targetPos = originHeadPos;
                targetPos.z = currentHeadPos.z;

                newHeadPos = Vector3.MoveTowards(currentHeadPos, targetPos, moveDist);
                line.SetPosition(lastIndex, newHeadPos);
            }
            else
            {
                newHeadPos = originHeadPos;
                line.SetPosition(lastIndex, originHeadPos);
            }

            int countCurrent = line.positionCount;
            int countOrigin = positionsOrigin.Length;
            int targetIndex = countOrigin - countCurrent;

            if (targetIndex >= 0)
            {
                Vector3 currentTailPos = line.GetPosition(0);
                Vector3 targetTailPos = positionsOrigin[targetIndex];

                if (Vector2.Distance(currentTailPos, targetTailPos) > 0.1f)
                {
                    Vector3 newTailPos = Vector3.MoveTowards(currentTailPos, targetTailPos, moveDist);

                    newTailPos.z = currentTailPos.z;

                    line.SetPosition(0, newTailPos);
                }
                else
                {
                    targetTailPos.z = currentTailPos.z;
                    line.SetPosition(0, targetTailPos);

                    if (targetIndex > 0)
                    {
                        int newCount = countCurrent + 1;
                        if (_arrayPool != null)
                            _tempPositionsArray = _arrayPool.GetArray(newCount);
                        else
                            _tempPositionsArray = new Vector3[newCount];

                        _tempPositionsArray[0] = targetTailPos;

                        for (int i = 0; i < countCurrent; i++)
                        {
                            _tempPositionsArray[i + 1] = line.GetPosition(i);
                        }

                        line.positionCount = newCount;
                        line.SetPositions(_tempPositionsArray);

                        if (_arrayPool != null)
                        {
                            _arrayPool.RecycleArray(_tempPositionsArray);
                            _tempPositionsArray = null;
                        }
                    }
                    else
                    {
                        if (Vector2.Distance(newHeadPos, originHeadPos) < 0.01f)
                        {
                            _isPlaying = false;
                            enabled = false;
                            OnAnimationCompleted?.Invoke();
                        }
                    }
                }
                OnLinePositionsChanged?.Invoke();
            }
        }
    }
}
