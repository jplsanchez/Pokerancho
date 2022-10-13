using Controller;
using System;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public delegate void PressButtonAction(Key key);
    public static event PressButtonAction ButtonPressed;

    private void Update()
    {
        CheckKeyboardInput();
    }

    private static void CheckKeyboardInput()
    {
        if (Input.GetKeyDown((KeyCode)Key.A_Button) && ButtonPressed != null) ButtonPressed(Key.A_Button);
        if (Input.GetKeyDown((KeyCode)Key.B_Button) && ButtonPressed != null) ButtonPressed(Key.B_Button);
        if (Input.GetKeyDown((KeyCode)Key.Up) && ButtonPressed != null) ButtonPressed(Key.Up);
        if (Input.GetKeyDown((KeyCode)Key.Down) && ButtonPressed != null) ButtonPressed(Key.Down);
        if (Input.GetKeyDown((KeyCode)Key.Left) && ButtonPressed != null) ButtonPressed(Key.Left);
        if (Input.GetKeyDown((KeyCode)Key.Right) && ButtonPressed != null) ButtonPressed(Key.Right);
    }

    public void PressButton(Key key)
    {
        ButtonPressed?.Invoke(key);
    }

    public void PressButton(string keyString)
    {
        if(Enum.TryParse(keyString, out Key key)) ButtonPressed?.Invoke(key);
    }


}
