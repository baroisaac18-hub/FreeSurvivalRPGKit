using UnityEngine;

public class MobileTouchControls : MonoBehaviour
{
    public static MobileTouchControls Instance { get; private set; }

    [Header("Joysticks")]
    public VirtualJoystick moveJoystick;
    public VirtualJoystick cameraJoystick;

    [Header("Buttons")]
    public TouchButton interactButton;
    public TouchButton jumpButton;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    public static float GetAxis(string axisName)
    {
        if (Instance == null) return 0f;
        switch (axisName)
        {
            case "Horizontal": return Instance.moveJoystick != null ? Instance.moveJoystick.Horizontal : 0f;
            case "Vertical": return Instance.moveJoystick != null ? Instance.moveJoystick.Vertical : 0f;
            case "Joystick Camera Rotate": return Instance.cameraJoystick != null ? Instance.cameraJoystick.Horizontal : 0f;
            default: return 0f;
        }
    }

    public static bool GetButtonDown(string buttonName)
    {
        if (Instance == null) return false;
        switch (buttonName)
        {
            case "Interact": return Instance.interactButton != null && Instance.interactButton.WasPressed;
            case "Jump": return Instance.jumpButton != null && Instance.jumpButton.WasPressed;
            default: return false;
        }
    }

    public static bool GetButton(string buttonName)
    {
        if (Instance == null) return false;
        switch (buttonName)
        {
            case "Interact": return Instance.interactButton != null && Instance.interactButton.IsPressed;
            case "Jump": return Instance.jumpButton != null && Instance.jumpButton.IsPressed;
            default: return false;
        }
    }

    public static bool GetMouseButton(int button)
    {
        if (button == 0 && Instance != null && Instance.interactButton != null && Instance.interactButton.IsPressed) return true;
        return Input.GetMouseButton(button);
    }

    public static bool GetMouseButtonDown(int button)
    {
        if (button == 0 && Instance != null && Instance.interactButton != null && Instance.interactButton.WasPressed) return true;
        return Input.GetMouseButtonDown(button);
    }

    public static Vector3 MousePosition
    {
        get { return Input.mousePosition; }
    }

    public static string[] JoystickNames { get { return new string[0]; } }
}
