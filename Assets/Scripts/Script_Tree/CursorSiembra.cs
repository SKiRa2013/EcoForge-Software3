using UnityEngine;
using UnityEngine.InputSystem;

public class CursorSiembra : MonoBehaviour
{
    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        transform.position = mouseScreen;
    }
}