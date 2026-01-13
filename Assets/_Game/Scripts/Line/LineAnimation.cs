using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Line
{
    public class LineAnimation : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private float speed = 5f;

    private bool _isPlaying;
    private bool _forward;
    private Vector3 _direction;
    [FormerlySerializedAs("positions")] private Vector3[] positionsOrigin;
    private bool _isInitialized;
    private bool _wasPlaying;
    private Vector3[] _tempPositionsArray;
    private static Vector3ArrayPool _arrayPool;

    public bool IsPlaying => _isPlaying;
    public bool IsForward => _forward;
    public Vector3 Direction => _direction;

    public event Action<bool> OnAnimationStarted;
    public event Action OnAnimationStopped;
    public event Action OnAnimationCompleted;
    public event Action OnLinePositionsChanged;

    public void Initialize(LineRenderer lineRenderer)
    {
        if (lineRenderer == null) return;

        line = lineRenderer;
        
        var count = line.positionCount;
        if (count < 2) return;
        
        positionsOrigin = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            positionsOrigin[i] = line.GetPosition(i);
        }

        var lastPoint = line.GetPosition(count - 1);
        _direction = lastPoint - line.GetPosition(count - 2);
        
        if (_arrayPool == null)
        {
            _arrayPool = FindFirstObjectByType<Vector3ArrayPool>();
            if (_arrayPool == null)
            {
                GameObject poolObj = new GameObject(nameof(Vector3ArrayPool));
                _arrayPool = poolObj.AddComponent<Vector3ArrayPool>();
            }
        }
        
        _isInitialized = true;
    }

    public void Play(bool forwardDirection)
    {
        if (!_isInitialized || line == null || line.positionCount < 2)
            return;
        
        bool wasPlaying = _isPlaying;
        _forward = forwardDirection;
        _isPlaying = true;

        if (!wasPlaying)
        {
            OnAnimationStarted?.Invoke(forwardDirection);
        }
    }


    public void Stop()
    {
        if (_isPlaying)
        {
            _isPlaying = false;
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
        bool wasPlaying = _wasPlaying;
        _wasPlaying = _isPlaying;

        if (!_isPlaying)
        {
            if (wasPlaying)
            {
                OnAnimationStopped?.Invoke();
            }
            return;
        }

        if (!line || line.positionCount < 2)
        {
            _isPlaying = false;
            if (wasPlaying)
            {
                OnAnimationStopped?.Invoke();
            }
            return;
        }

        if (_forward)
            AnimateForward();
        else
            AnimateBackward();
    }

    private void AnimateForward()
    {
        var count = line.positionCount;
        var lastPoint = line.GetPosition(count - 1);

        lastPoint += _direction.normalized * (speed * Time.deltaTime);
        line.SetPosition(count - 1, lastPoint);

        var tailPoint = line.GetPosition(0);
        var tailDirection = line.GetPosition(1) - tailPoint;
        tailPoint += tailDirection.normalized * (speed * Time.deltaTime);
        line.SetPosition(0, tailPoint);

        OnLinePositionsChanged?.Invoke();

        if (!(Vector3.Distance(tailPoint, line.GetPosition(1)) < 0.1f)) return;

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
            OnAnimationCompleted?.Invoke();
        }
    }

    private void AnimateBackward()
    {
        var countCurrent = line.positionCount;
        var countOrigin = positionsOrigin.Length;

        if (countCurrent < countOrigin)
        {
            var newCount = countCurrent + 1;
            if (_arrayPool != null)
            {
                _tempPositionsArray = _arrayPool.GetArray(newCount);
            }
            else
            {
                _tempPositionsArray = new Vector3[newCount];
            }

            var indexTarget = countOrigin - newCount;
            _tempPositionsArray[0] = positionsOrigin[indexTarget];
            _tempPositionsArray[1] = positionsOrigin[indexTarget];
            for (int i = 1; i < countCurrent; i++)
            {
                _tempPositionsArray[i + 1] = line.GetPosition(i);
            }

            line.positionCount = newCount;
            line.SetPositions(_tempPositionsArray);
            
            if (_arrayPool != null)
            {
                _arrayPool.RecycleArray(_tempPositionsArray);
            }
            _tempPositionsArray = null;
            
            OnLinePositionsChanged?.Invoke();
            return;
        }

        bool allPositionsClose = true;
        for (int i = 0; i < countCurrent && i < countOrigin; i++)
        {
            var currentPos = line.GetPosition(i);
            var originPos = positionsOrigin[i];
            if (Vector3.Distance(currentPos, originPos) > 0.1f)
            {
                allPositionsClose = false;
                break;
            }
        }

        if (allPositionsClose && countCurrent == countOrigin)
        {
            line.SetPositions(positionsOrigin);
            _isPlaying = false;
            OnLinePositionsChanged?.Invoke();
            OnAnimationCompleted?.Invoke();
            return;
        }

        var positionHeadOrigin = positionsOrigin[positionsOrigin.Length - 1];
        var indexHead = countCurrent - 1;
        var positionHead = line.GetPosition(indexHead);
        var directionToOrigin = positionHeadOrigin - positionHead;
        
        if (Vector3.Distance(positionHeadOrigin, positionHead) > 0.1f)
        {
            positionHead += directionToOrigin.normalized * (speed * Time.deltaTime);
            
            if (Vector3.Dot(directionToOrigin, positionHeadOrigin - positionHead) < 0)
            {
                positionHead = positionHeadOrigin;
            }
            
            line.SetPosition(indexHead, positionHead);
        }
        else
        {
            line.SetPosition(indexHead, positionHeadOrigin);
        }

        var positionTail = line.GetPosition(0);
        var positionTailTarget = positionsOrigin[0];
        var directionTail = positionTailTarget - positionTail;

        if (Vector3.Distance(positionTail, positionTailTarget) > 0.1f)
        {
            positionTail += directionTail.normalized * (speed * Time.deltaTime);
            line.SetPosition(0, positionTail);
        }
        else
        {
            line.SetPosition(0, positionTailTarget);
        }

        for (int i = 1; i < countCurrent - 1 && i < countOrigin - 1; i++)
        {
            var currentPos = line.GetPosition(i);
            var targetPos = positionsOrigin[i];
            var dir = targetPos - currentPos;
            
            if (Vector3.Distance(currentPos, targetPos) > 0.1f)
            {
                currentPos += dir.normalized * (speed * Time.deltaTime);
                line.SetPosition(i, currentPos);
            }
            else
            {
                line.SetPosition(i, targetPos);
            }
        }

        OnLinePositionsChanged?.Invoke();
    }

}
}