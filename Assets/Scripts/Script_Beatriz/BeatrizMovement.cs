using UnityEngine;
using UnityEngine.InputSystem;

public class BeatrizMovement : MonoBehaviour
{
    private float speed = 2f;

    private Rigidbody2D BeatrizRb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection;

    // ── Control externo del movimiento ──
    private bool movimientoActivo = true;

    PlayerInput playerInput;
    InputAction moverArribaAction;
    InputAction moverAbajoAction;
    InputAction moverIzquierdaAction;
    InputAction moverDerechaAction;

    void Start()
    {
        BeatrizRb = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();

        playerInput          = GetComponent<PlayerInput>();
        moverArribaAction    = playerInput.actions["MoverArriba"];
        moverAbajoAction     = playerInput.actions["MoverAbajo"];
        moverIzquierdaAction = playerInput.actions["MoverIzquierda"];
        moverDerechaAction   = playerInput.actions["MoverDerecha"];
    }

    void Update()
    {
        // Si el movimiento está desactivado → detener
        if (!movimientoActivo)
        {
            moveInput = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        moveInput = Vector2.zero;

        if (moverArribaAction.IsPressed())    moveInput.y =  1;
        if (moverAbajoAction.IsPressed())     moveInput.y = -1;
        if (moverIzquierdaAction.IsPressed()) moveInput.x = -1;
        if (moverDerechaAction.IsPressed())   moveInput.x =  1;

        moveInput = moveInput.normalized;

        if (moveInput != Vector2.zero)
            lastMoveDirection = moveInput;

        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical",   lastMoveDirection.y);
        animator.SetFloat("Speed",      moveInput.magnitude);
    }

    private void FixedUpdate()
    {
        if (!movimientoActivo) return;
        BeatrizRb.MovePosition(BeatrizRb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    // ── Método público para pausar/reanudar movimiento ──
    public void SetMovimientoActivo(bool activo)
    {
        movimientoActivo = activo;

        if (!activo)
        {
            moveInput = Vector2.zero;
            BeatrizRb.linearVelocity = Vector2.zero;
        }
    }
}