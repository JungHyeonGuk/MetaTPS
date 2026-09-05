using UnityEngine;
using UnityEngine.EventSystems;

public class MobilePointerZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public System.Action<PointerEventData> Pressed;
    public System.Action<PointerEventData> Dragged;
    public System.Action<PointerEventData> Released;

    public void OnPointerDown(PointerEventData eventData) => Pressed?.Invoke(eventData);
    public void OnDrag(PointerEventData eventData) => Dragged?.Invoke(eventData);
    public void OnPointerUp(PointerEventData eventData) => Released?.Invoke(eventData);
}
