using UnityEngine;
using UnityEngine.InputSystem;

public class Seeds : MonoBehaviour
{
    [SerializeField] private TypeSeed type;
    [SerializeField] private ContenedorSeedUI contenedor;

    private bool jugadorDentro;

    void Update()
    {
        if (jugadorDentro && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Recolectar();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = false;
        }
    }

    private void Recolectar()
    {
        if (contenedor != null)
        {
            contenedor.AgregarSeed(type);
        }

        Destroy(gameObject);
    }
}

public enum TypeSeed
{
    Comun,
    Red,
    Green,
    Blue,
    Golden
}