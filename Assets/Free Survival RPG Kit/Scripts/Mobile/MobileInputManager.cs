// MobileInputManager.cs
// Static input abstraction: on Android reads virtual joystick/touch from MobileTouchControls,
// on PC falls back to legacy UnityEngine.Input. No game logic is rewritten.
using UnityEngine;

public static class MobileInputManager
{
    public static bool IsMobile()
    {
#if UNITY_ANDROID || UNITY_IOS
        return true;
#else
        return Application.isMobilePlatform;
#endif
    }

    public static float GetAxis(string name) { return GetAxisRaw(name); }
    public static float GetAxisRaw(string name)
    {
        if (IsMobile() && MobileTouchControls.Instance != null)
            return MobileTouchControls.Instance.GetAxis(name);
        return Input.GetAxisRaw(name);
    }

    public static bool GetButtonDown(string name)
    {
        if (IsMobile() && MobileTouchControls.Instance != null)
            return MobileTouchControls.Instance.GetButtonDown(name);
        return Input.GetButtonDown(name);
    }

    public static bool GetMouseButton(int button)
    {
        if (IsMobile() && MobileTouchControls.Instance != null)
            return MobileTouchControls.Instance.GetMouseButton(button);
        return Input.GetMouseButton(button);
    }
    public static bool GetMouseButtonDown(int button)
    {
        if (IsMobile() && MobileTouchControls.Instance != null)
            return MobileTouchControls.Instance.GetMouseButtonDown(button);
        return Input.GetMouseButtonDown(button);
    }
    public static Vector3 mousePosition
    {
        get
        {
            if (IsMobile() && MobileTouchControls.Instance != null)
                return MobileTouchControls.Instance.MousePosition();
            return Input.mousePosition;
        }
    }

    public static string[] GetJoystickNames()
    {
        if (IsMobile() && MobileTouchControls.Instance != null)
            return MobileTouchControls.Instance.JoystickNames();
        return Input.GetJoystickNames();
    }
}
