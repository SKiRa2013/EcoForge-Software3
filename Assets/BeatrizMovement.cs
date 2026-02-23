using UnityEngine;
using UnityEngine.InputSystem;

public class BeatrizMovement : MonoBehaviour
{
    private float speed = 4f;

    private Rigidbody2D BeatrizRb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection; // Para mantener dirección cuando esté quieta

    void Start()
    {
        BeatrizRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            moveInput = Vector2.zero;

            if (Keyboard.current.wKey.isPressed)
                moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed)
                moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed)
                moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed)
                moveInput.x = 1;

            // Normalizar para que diagonal no sea más rápida
            moveInput = moveInput.normalized;

            // Guardar última dirección válida
            if (moveInput != Vector2.zero)
            {
                lastMoveDirection = moveInput;
            }

            // 🔹 Actualizar parámetros del Animator
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
            animator.SetFloat("speed", moveInput.magnitude);
        }
    }

    private void FixedUpdate()
    {
        BeatrizRb.MovePosition(BeatrizRb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}