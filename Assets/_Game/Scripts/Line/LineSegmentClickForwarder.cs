using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class LineSegmentClickForwarder : MonoBehaviour, IPointerClickHandler
{
    private LineClick _parentLineClick;

    private void Start()
    {
        _parentLineClick = GetComponentInParent<LineClick>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_parentLineClick != null)
        {
            _parentLineClick.OnPointerClick(eventData);
        }
    }
}
