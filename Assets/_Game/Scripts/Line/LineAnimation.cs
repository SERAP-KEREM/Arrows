using UnityEngine;
using UnityEngine.Serialization;

public class LineAnimation : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float speed = 5f;

    public bool play;
    public bool forward = true;

    public Vector3 direction;
    [FormerlySerializedAs("positions")] public Vector3[] positionsOrigin;

    private void Start()
    {
        if (line == null) return;
        
        var count = line.positionCount;
        if (count < 2) return;
        
        positionsOrigin = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            positionsOrigin[i] = line.GetPosition(i);
        }

        var lastPoint = line.GetPosition(count - 1);
        direction = lastPoint - line.GetPosition(count - 2);
    }

    public void Play(bool forwardDirection)
    {
        if (line == null || line.positionCount < 2)
            return;
        
        forward = forwardDirection;
        play = true;
    }

    public void Stop()
    {
        play = false;
    }

    private void Update()
    {
        if (!play) return;

        if (!line || line.positionCount < 2)
        {
            play = false;
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