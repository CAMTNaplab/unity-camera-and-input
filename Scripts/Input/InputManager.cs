﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    private static Dictionary<string, SimulateButton> simulateInputs = new Dictionary<string, SimulateButton>();
    private static Dictionary<string, SimulateAxis> simulateAxis = new Dictionary<string, SimulateAxis>();
    public static bool useMobileInputOnNonMobile = false;

    public static bool HasInputSetting()
    {
        return InputSettingManager.Singleton != null;
    }

    public static KeyCode GetInputKeyCode(string keyName)
    {
        if (!InputSettingManager.Singleton.Settings.ContainsKey(keyName))
            return KeyCode.None;
        return InputSettingManager.Singleton.Settings[keyName];
    }

    public static float GetAxis(string name, bool raw)
    {
        float axis = 0;
        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
        {
            if (axis == 0 && simulateAxis.ContainsKey(name))
                axis = simulateAxis[name].GetValue;
        }
        else
        {
            axis = raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }
        return axis;
    }

    public static bool GetButton(string name)
    {
        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
            return (simulateInputs.ContainsKey(name) && simulateInputs[name].GetButton);
        if (HasInputSetting())
            return Input.GetKey(GetInputKeyCode(name));
        return Input.GetButton(name);
    }

    public static bool GetButtonDown(string name)
    {
        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
            return (simulateInputs.ContainsKey(name) && simulateInputs[name].GetButtonDown);
        if (HasInputSetting())
            return Input.GetKeyDown(GetInputKeyCode(name));
        return Input.GetButtonDown(name);
    }

    public static bool GetButtonUp(string name)
    {
        if (useMobileInputOnNonMobile || Application.isMobilePlatform)
            return (simulateInputs.ContainsKey(name) && simulateInputs[name].GetButtonUp);
        if (HasInputSetting())
            return Input.GetKeyUp(GetInputKeyCode(name));
        return Input.GetButtonUp(name);
    }

    public static void SetButtonDown(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            var inputData = new SimulateButton();
            simulateInputs.Add(name, inputData);
        }
        simulateInputs[name].Press();
    }

    public static void SetButtonUp(string name)
    {
        if (!simulateInputs.ContainsKey(name))
        {
            var inputData = new SimulateButton();
            simulateInputs.Add(name, inputData);
        }
        simulateInputs[name].Release();
    }

    public static void SetAxisPositive(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            var inputData = new SimulateAxis();
            simulateAxis.Add(name, inputData);
        }
        simulateAxis[name].Update(1f);
    }
    
    public static void SetAxisNegative(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            var inputData = new SimulateAxis();
            simulateAxis.Add(name, inputData);
        }
        simulateAxis[name].Update(-1f);
    }

    public static void SetAxisZero(string name)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            var inputData = new SimulateAxis();
            simulateAxis.Add(name, inputData);
        }
        simulateAxis[name].Update(0);
    }
    
    public static void SetAxis(string name, float value)
    {
        if (!simulateAxis.ContainsKey(name))
        {
            var inputData = new SimulateAxis();
            simulateAxis.Add(name, inputData);
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
        private bool pressed;

        public void Press()
        {
            if (pressed)
                return;
            pressed = true;
            lastPressedFrame = Time.frameCount;
        }

        public void Release()
        {
            pressed = false;
            releasedFrame = Time.frameCount;
        }

        public bool GetButton
        {
            get { return pressed; }
        }

        public bool GetButtonDown
        {
            get { return lastPressedFrame - Time.frameCount == -1; }
        }

        public bool GetButtonUp
        {
            get { return (releasedFrame == Time.frameCount - 1); }
        }
    }

    public class SimulateAxis
    {
        private float value;

        public void Update(float value)
        {
            this.value = value;
        }

        public float GetValue
        {
            get { return value; }
        }
    }
}
