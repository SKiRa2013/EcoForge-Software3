using UnityEngine;
using UnityEngine.InputSystem;

public class Seeds : MonoBehaviour
{
    [SerializeField] private TypeSeed type;
    [SerializeField] private ContenedorSeedUI contenedor;

    private bool jugadorDentro;

    PlayerInput playerInput;
    InputAction interactuarAction;

    void Start()
    {
        playerInput       = FindFirstObjectByType<PlayerInput>();
        interactuarAction = playerInput.actions["Interactuar"];
    }

    void Update()
    {
        if (jugadorDentro && interactuarAction.WasPressedThisFrame())
            Recolectar();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = false;
    }

    private void Recolectar()
    {
        if (contenedor.ObtenerCantidad(type) >= 6)
        {
            Debug.Log("Inventario lleno para " + type);
            return;
        }
        contenedor.AgregarSeed(type);
        Destroy(gameObject);
    }
}