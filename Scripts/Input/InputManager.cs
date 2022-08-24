﻿using System.Collections.Generic;
using UnityEngine;
#if USE_REWIRED
using Rewired;
#endif

public static class InputManager
{
    private static Dictionary<string, SimulateButton> simulateInputs = new Dictionary<string, SimulateButton>();
    private static Dictionary<KeyCode, SimulateButton> simulateKeys = new Dictionary<KeyCode, SimulateButton>();
    private static Dictionary<string, SimulateAxis> simulateAxis = new Dictionary<string, SimulateAxis>();
    public static bool useMobileInputOnNonMobile = false;
    public static bool useNonMobileInput = false;

    public static bool HasInputSetting(string keyName)
    {
        return InputSettingManager.Singleton != null && InputSettingManager.Singleton.Settings.ContainsKey(keyName);
    }

    public static bool UseMobileInput()
    {
#if VR_BUILD
        return false;
#else
        return Application.isMobilePlatform || useMobileInputOnNonMobile;
#endif
    }

    public static bool UseNonMobileInput()
    {
#if VR_BUILD
        return true;
#else
        return !Application.isMobilePlatform && (!useMobileInputOnNonMobile || useNonMobileInput);
#endif
    }

#if USE_REWIRED
    public static float GetAxis(string name, bool raw, int playerId = 0)
#else
    public static float GetAxis(string name, bool raw)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return raw ? player.GetAxisRaw(name) : player.GetAxis(name);
        }
        catch { }
#endif

        if (UseNonMobileInput())
        {
            try
            {
                return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
            }
            catch { }
        }

        if (UseMobileInput())
        {
            SimulateAxis foundSimulateAxis;
            float axis = 0f;
            if (simulateAxis.TryGetValue(name, out foundSimulateAxis))
                axis = foundSimulateAxis.Value;
            if (raw)
            {
                if (axis > 0f)
                    axis = 1f;
                if (axis < 0f)
                    axis = -1f;
            }
            return axis;
        }
        return 0f;
    }

    public static bool GetKey(KeyCode key)
    {
        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetKey(key))
                    return true;
            }
            catch { }
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateKeys.TryGetValue(key, out foundSimulateButton) && foundSimulateButton.Pressed)
                return true;
        }
        return false;
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetKeyDown(key))
                    return true;
            }
            catch { }
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateKeys.TryGetValue(key, out foundSimulateButton) && foundSimulateButton.ButtonDown)
                return true;
        }
        return false;
    }

    public static bool GetKeyUp(KeyCode key)
    {
        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetKeyUp(key))
                    return true;
            }
            catch { }
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateKeys.TryGetValue(key, out foundSimulateButton) && foundSimulateButton.ButtonUp)
                return true;
        }
        return false;
    }

    private static bool IsKeyFromSettingActivated(string name, System.Func<KeyCode, bool> func)
    {
        if (HasInputSetting(name))
        {
            List<KeyCode> keyCodes = InputSettingManager.Singleton.Settings[name];
            foreach (KeyCode keyCode in keyCodes)
            {
                if (func.Invoke(keyCode))
                    return true;
            }
        }
        return false;
    }

#if USE_REWIRED
    public static bool GetButton(string name, int playerId = 0)
#else
    public static bool GetButton(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButton(name);
        }
        catch { }
#endif

        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetButton(name))
                    return true;
            }
            catch { }
            if (IsKeyFromSettingActivated(name, GetKey))
                return true;
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.Pressed)
                return true;
            if (!UseNonMobileInput())
            {
                if (IsKeyFromSettingActivated(name, GetKey))
                    return true;
            }
        }
        return false;
    }

#if USE_REWIRED
    public static bool GetButtonDown(string name, int playerId = 0)
#else
    public static bool GetButtonDown(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButtonDown(name);
        }
        catch { }
#endif

        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetButtonDown(name))
                    return true;
            }
            catch { }
            if (IsKeyFromSettingActivated(name, GetKeyDown))
                return true;
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.ButtonDown)
                return true;
            if (!UseNonMobileInput())
            {
                if (IsKeyFromSettingActivated(name, GetKeyDown))
                    return true;
            }
        }
        return false;
    }

#if USE_REWIRED
    public static bool GetButtonUp(string name, int playerId = 0)
#else
    public static bool GetButtonUp(string name)
#endif
    {
        // Try get input by rewired system
#if USE_REWIRED
        try
        {
            Player player = ReInput.players.GetPlayer(playerId);
            return player.GetButtonUp(name);
        }
        catch { }
#endif

        if (UseNonMobileInput())
        {
            try
            {
                if (Input.GetButtonUp(name))
                    return true;
            }
            catch { }
            if (IsKeyFromSettingActivated(name, GetKeyUp))
                return true;
        }

        if (UseMobileInput())
        {
            SimulateButton foundSimulateButton;
            if (simulateInputs.TryGetValue(name, out foundSimulateButton) && foundSimulateButton.ButtonUp)
                return true;
            if (!UseNonMobileInput())
            {
                if (IsKeyFromSettingActivated(name, GetKeyUp))
                    return true;
            }
        }
        return false;
    }

    public static void SetButtonDown(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            simulateInputs.Add(name, new SimulateButton());
        }
        simulateInputs[name].Press();
    }

    public static void SetButtonUp(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            simulateInputs.Add(name, new SimulateButton());
        }
        simulateInputs[name].Release();
    }

    public static void SetKeyDown(KeyCode key)
    {
        if (!simulateKeys.ContainsKey(key))
        {
            simulateKeys.Add(key, new SimulateButton());
        }
        simulateKeys[key].Press();
    }

    public static void SetKeyUp(KeyCode key)
    {
        if (!simulateKeys.ContainsKey(key))
        {
            simulateKeys.Add(key, new SimulateButton());
        }
        simulateKeys[key].Release();
    }

    public static void SetAxisPositive(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(1f);
    }

    public static void SetAxisNegative(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(-1f);
    }

    public static void SetAxisZero(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(0);
    }

    public static void SetAxis(string name, float value)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            simulateAxis.Add(name, new SimulateAxis());
        }
        simulateAxis[name].Update(value);
    }

    public static Vector3 MousePosition()
    {
        return Input.mousePosition;
    }

    public class SimulateButton
    {
        private int lastPressedFrame = -5;
        private int releasedFrame = -5;

        public bool Pressed { get; private set; }

        public void Press()
        {
            if (Pressed)
                return;
            Pressed = true;
            lastPressedFrame = Time.frameCount;
        }

        public void Release()
        {
            Pressed = false;
            releasedFrame = Time.frameCount;
        }

        public bool ButtonDown
        {
            get { return lastPressedFrame - Time.frameCount == -1; }
        }

        public bool ButtonUp
        {
            get { return releasedFrame == Time.frameCount - 1; }
        }
    }

    public class SimulateAxis
    {
        public float Value { get; private set; }
        public void Update(float value)
        {
            Value = value;
        }
    }
}
