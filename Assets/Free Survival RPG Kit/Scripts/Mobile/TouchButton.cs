using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }
    public bool WasPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData) { IsPressed = true; WasPressed = true; }
    public void OnPointerUp(PointerEventData eventData) { IsPressed = false; }

    void LateUpdate() { WasPressed = false; }
}
