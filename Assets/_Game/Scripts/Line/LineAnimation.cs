using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LineAnimation : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private float speed = 5f;

    public bool play;
    public bool forward = true;

    public Vector3 direction;
    [FormerlySerializedAs("positions")] public Vector3[] positionsOrigin;
    private bool _isInitialized;
    private bool _wasPlaying;

    public event Action<bool> OnAnimationStarted;
    public event Action OnAnimationStopped;
    public event Action OnAnimationCompleted;

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
        direction = lastPoint - line.GetPosition(count - 2);
        
        _isInitialized = true;
    }

    public void Play(bool forwardDirection)
    {
        if (!_isInitialized || line == null || line.positionCount < 2)
            return;
        
        bool wasPlaying = play;
        forward = forwardDirection;
        play = true;

        if (!wasPlaying)
        {
            OnAnimationStarted?.Invoke(forwardDirection);
        }
    }

    public void Stop()
    {
        if (play)
        {
            play = false;
            OnAnimationStopped?.Invoke();
        }
    }

    private void Update()
    {
        bool wasPlaying = _wasPlaying;
        _wasPlaying = play;

        if (!play)
        {
            if (wasPlaying)
            {
                OnAnimationStopped?.Invoke();
            }
            return;
        }

        if (!line || line.positionCount < 2)
        {
            play = false;
            if (wasPlaying)
            {
                OnAnimationStopped?.Invoke();
            }
            return;
        }

        if (forward)
            AnimateForward();
        else
            AnimateBackward();
    }

    private void AnimateForward()
    {
        var count = line.positionCount;
        var lastPoint = line.GetPosition(count - 1);

        lastPoint += direction.normalized * (speed * Time.deltaTime);
        line.SetPosition(count - 1, lastPoint);

        var tailPoint = line.GetPosition(0);
        var tailDirection = line.GetPosition(1) - tailPoint;
        tailPoint += tailDirection.normalized * (speed * Time.deltaTime);
        line.SetPosition(0, tailPoint);

        if (!(Vector3.Distance(tailPoint, line.GetPosition(1)) < 0.1f)) return;

        var newCount = count - 1;
        var newPositions = new Vector3[newCount];
        for (int i = 1; i < count; i++)
        {
            newPositions[i - 1] = line.GetPosition(i);
        }

        line.positionCount = newCount;
        line.SetPositions(newPositions);

        if (newCount < 2)
        {
            play = false;
            OnAnimationCompleted?.Invoke();
        }
    }

    private void AnimateBackward()
    {
        var countCurrent = line.positionCount;
        var countOrigin = positionsOrigin.Length;

        var tailPosition = line.GetPosition(0);
        var originTailPosition = positionsOrigin[0];
        var isSameCount = countCurrent >= countOrigin;

        if (isSameCount && Vector3.Distance(tailPosition, originTailPosition) < 0.1f)
        {
            play = false;
            OnAnimationCompleted?.Invoke();
            return;
        }

        var positionHeadOrigin = positionsOrigin[positionsOrigin.Length - 1];
        var indexHead = countCurrent - 1;
        var positionHead = line.GetPosition(indexHead);

        if (Vector3.Distance(positionHeadOrigin, positionHead) > 0.1f || !isSameCount)
        {
            positionHead -= direction.normalized * (speed * Time.deltaTime);
            line.SetPosition(indexHead, positionHead);
        }
        else
        {
            line.SetPosition(indexHead, positionHeadOrigin);
        }

        var indexTarget = countOrigin - countCurrent;
        var positionTail = line.GetPosition(0);
        var positionTailTarget = positionsOrigin[indexTarget];
        var directionTail = positionTailTarget - positionTail;

        positionTail += directionTail.normalized * (speed * Time.deltaTime);
        line.SetPosition(0, positionTail);

        if (Vector3.Distance(positionTail, positionsOrigin[indexTarget]) >= 0.1f)
            return;

        var newCount = countCurrent + 1;
        var newPositions = new Vector3[newCount];

        newPositions[0] = positionsOrigin[indexTarget];
        newPositions[1] = positionsOrigin[indexTarget];
        for (int i = 1; i < countCurrent; i++)
        {
            newPositions[i + 1] = line.GetPosition(i);
        }

        line.positionCount = newCount;
        line.SetPositions(newPositions);
    }
}