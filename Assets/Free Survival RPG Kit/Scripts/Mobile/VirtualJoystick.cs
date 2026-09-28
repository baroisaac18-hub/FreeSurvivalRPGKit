using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float range = 100f;

    private Vector2 inputVec;
    private float zoom;
    public bool IsDragging { get; private set; }
    public bool WasDragStarted { get; private set; }

    public float Horizontal { get { return inputVec.x; } }
    public float Vertical { get { return inputVec.y; } }
    public float Zoom { get { return zoom; } }

    void Awake()
    {
        if (background == null) background = transform as RectTransform;
        if (handle == null && transform.childCount > 0) handle = transform.GetChild(0) as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragging = true;
        WasDragStarted = true;
        UpdateInput(eventData);
    }

    public void OnDrag(PointerEventData eventData) { UpdateInput(eventData); }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDragging = false;
        inputVec = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }

    private void UpdateInput(PointerEventData eventData)
    {
        if (background != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out Vector2 pos))
        {
            float r = background.rect.width * 0.5f;
            if (r <= 0f) r = range;
            inputVec = Vector2.ClampMagnitude(pos / r, 1f);
            if (handle != null) handle.anchoredPosition = inputVec * r;
        }
        if (Input.touchCount >= 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            float prev = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float cur = (t0.position - t1.position).magnitude;
            zoom = prev > 0f ? (cur - prev) / prev : 0f;
        }
        else zoom = 0f;
    }
}
