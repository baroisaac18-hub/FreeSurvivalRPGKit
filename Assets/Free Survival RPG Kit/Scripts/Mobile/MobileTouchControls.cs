using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Auto-creates mobile touch UI at runtime: left joystick, right camera drag area,
// action buttons (Interact/Jump/Inventory/Unequip/Minimap). Works on both PC and Android.
public class MobileTouchControls : MonoBehaviour
{
    public static MobileTouchControls Instance { get; private set; }

    public VirtualJoystick moveJoystick;
    public VirtualJoystick cameraJoystick;
    public TouchButton interactButton;
    public TouchButton jumpButton;
    public TouchButton inventoryButton;
    public TouchButton unequipButton;
    public TouchButton minimapZoomIn;
    public TouchButton minimapZoomOut;

    Canvas canvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (!MobileInputManager.IsMobile()) return;
        if (Instance != null) return;
        var go = new GameObject("MobileTouchControls");
        go.AddComponent<MobileTouchControls>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureEventSystem();
        CreateCanvas();
        CreateJoysticks();
        CreateButtons();
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    void EnsureEventSystem()
    {
        if (EventSystem.current == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(es);
        }
    }

    void CreateCanvas()
    {
        var go = new GameObject("MobileTouchCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        var sc = go.GetComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920f, 1080f);
        sc.matchWidthOrHeight = 0.5f;
        DontDestroyOnLoad(go);
    }

    RectTransform MakeRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return rt;
    }

    VirtualJoystick MakeJoystick(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, float size, bool withHandle)
    {
        var rt = MakeRect(name, parent, anchorMin, anchorMax, pos, new Vector2(size, size));
        var img = rt.gameObject.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.20f);
        img.raycastTarget = true;
        var js = rt.gameObject.AddComponent<VirtualJoystick>();
        if (withHandle)
        {
            var hrt = MakeRect("Handle", rt, new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), Vector2.zero, new Vector2(size*0.45f, size*0.45f));
            var himg = hrt.gameObject.AddComponent<Image>();
            himg.color = new Color(1f, 1f, 1f, 0.45f);
            himg.raycastTarget = false;
            js.handle = hrt;
        }
        return js;
    }

    TouchButton MakeButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, float size, string label, Color color)
    {
        var rt = MakeRect(name, parent, anchorMin, anchorMax, pos, new Vector2(size, size));
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;
        var btn = rt.gameObject.AddComponent<TouchButton>();

        var trt = MakeRect("Label", rt, new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), Vector2.zero, new Vector2(size, size));
        var txt = trt.gameObject.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = (int)(size * 0.32f);
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.raycastTarget = false;
        return btn;
    }

    void CreateJoysticks()
    {
        // left move joystick (bottom-left)
        moveJoystick = MakeJoystick("MoveJoystick", canvas.transform, new Vector2(0f,0f), new Vector2(0f,0f), new Vector2(220f, 220f), 280f, true);
        // right camera drag area (right half screen)
        cameraJoystick = MakeJoystick("CameraArea", canvas.transform, new Vector2(0.5f,0f), new Vector2(1f,1f), new Vector2(0f,0f), 10f, false);
        var ci = cameraJoystick.GetComponent<Image>();
        if (ci != null) ci.color = new Color(1f,1f,1f,0.01f);
    }

    void CreateButtons()
    {
        interactButton = MakeButton("Interact", canvas.transform, new Vector2(1f,0f), new Vector2(1f,0f), new Vector2(-160f, 180f), 150f, "X", new Color(0.13f, 0.55f, 0.95f, 0.8f));
        jumpButton = MakeButton("Jump", canvas.transform, new Vector2(1f,0f), new Vector2(1f,0f), new Vector2(-320f, 320f), 110f, "A", new Color(0.36f, 0.81f, 0.25f, 0.8f));
        inventoryButton = MakeButton("Inventory", canvas.transform, new Vector2(1f,1f), new Vector2(1f,1f), new Vector2(-120f, -110f), 90f, "I", new Color(0.4f, 0.4f, 0.4f, 0.8f));
        unequipButton = MakeButton("Unequip", canvas.transform, new Vector2(1f,1f), new Vector2(1f,1f), new Vector2(-220f, -110f), 80f, "U", new Color(0.5f, 0.3f, 0.3f, 0.8f));
        minimapZoomIn = MakeButton("ZoomIn", canvas.transform, new Vector2(1f,1f), new Vector2(1f,1f), new Vector2(-120f, -230f), 70f, "+", new Color(0.3f, 0.3f, 0.3f, 0.8f));
        minimapZoomOut = MakeButton("ZoomOut", canvas.transform, new Vector2(1f,1f), new Vector2(1f,1f), new Vector2(-200f, -230f), 70f, "-", new Color(0.3f, 0.3f, 0.3f, 0.8f));
    }

    // ---- API consumed by MobileInputManager ----
    public float GetAxis(string name)
    {
        switch (name)
        {
            case "Horizontal":
            case "Horizontal Joystick":
                return moveJoystick != null ? moveJoystick.Horizontal : 0f;
            case "Vertical":
            case "Vertical Joystick":
                return moveJoystick != null ? moveJoystick.Vertical : 0f;
            case "Joystick Camera Rotate":
            case "Mouse X":
                return cameraJoystick != null ? cameraJoystick.Horizontal : 0f;
            case "Mouse ScrollWheel":
            case "Mouse ScrollWheel Joystick":
                return cameraJoystick != null ? cameraJoystick.Zoom : 0f;
        }
        return 0f;
    }

    public bool GetButtonDown(string name)
    {
        switch (name)
        {
            case "Interact": return interactButton != null && interactButton.WasPressed;
            case "Jump": return jumpButton != null && jumpButton.WasPressed;
            case "Inventory": return inventoryButton != null && inventoryButton.WasPressed;
            case "Unequip": return unequipButton != null && unequipButton.WasPressed;
            case "MinimapZoom": return minimapZoomIn != null && minimapZoomIn.WasPressed;
            case "MinimapZoomOut": return minimapZoomOut != null && minimapZoomOut.WasPressed;
        }
        return false;
    }

    public bool GetButton(string name)
    {
        switch (name)
        {
            case "Interact": return interactButton != null && interactButton.IsPressed;
            case "Jump": return jumpButton != null && jumpButton.IsPressed;
            case "Inventory": return inventoryButton != null && inventoryButton.IsPressed;
            case "Unequip": return unequipButton != null && unequipButton.IsPressed;
            case "MinimapZoom": return minimapZoomIn != null && minimapZoomIn.IsPressed;
            case "MinimapZoomOut": return minimapZoomOut != null && minimapZoomOut.IsPressed;
        }
        return false;
    }

    public bool GetMouseButton(int button)
    {
        if (button == 0 && interactButton != null && interactButton.IsPressed) return true;
        if (button == 1 && cameraJoystick != null && cameraJoystick.IsDragging) return true;
        return Input.GetMouseButton(button);
    }

    public bool GetMouseButtonDown(int button)
    {
        if (button == 0 && interactButton != null && interactButton.WasPressed) return true;
        if (button == 1 && cameraJoystick != null && cameraJoystick.WasDragStarted) return true;
        return Input.GetMouseButtonDown(button);
    }

    public Vector3 MousePosition() { return Input.mousePosition; }
    public string[] JoystickNames() { return new string[] { "MobileTouch" }; }
}
