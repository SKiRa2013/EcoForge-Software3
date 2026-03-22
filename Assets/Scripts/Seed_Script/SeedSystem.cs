using UnityEngine;
using UnityEngine.InputSystem;

public enum EstadoSiembra
{
    Inactivo,
    MenuAbierto,
    Sembrando
}

public class SeedSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BeatrizMovement movimiento;
    [SerializeField] private ContenedorSeedUI contenedor;
    [SerializeField] private ManaSystem manaSystem;

    [Header("UI")]
    [SerializeField] private GameObject panelMenuSemillas;
    [SerializeField] private GameObject panelSembrando;

    [Header("Prefabs de plantas")]
    [SerializeField] private GameObject plantaComun;
    [SerializeField] private GameObject plantaRed;
    [SerializeField] private GameObject plantaGreen;
    [SerializeField] private GameObject plantaBlue;
    [SerializeField] private GameObject plantaGolden;

    [Header("Control de espacio")]
    [SerializeField] private float radioChequeo = 0.4f;

    EstadoSiembra estado = EstadoSiembra.Inactivo;
    TypeSeed semillaSeleccionada = TypeSeed.Comun;

    PlayerInput playerInput;
    InputAction plantAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
            plantAction = playerInput.actions["Plant"];
    }

    void OnEnable()
    {
        if (plantAction != null)
            plantAction.performed += OnPlantPressed;
    }

    void OnDisable()
    {
        if (plantAction != null)
            plantAction.performed -= OnPlantPressed;
    }

    void Start()
    {
        if (manaSystem == null)
            manaSystem = FindFirstObjectByType<ManaSystem>();

        CerrarTodo();
    }

    void Update()
    {
        switch (estado)
        {
            case EstadoSiembra.MenuAbierto:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    CerrarTodo();
                break;

            case EstadoSiembra.Sembrando:
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    Plantar();

                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    CerrarTodo();
                break;
        }
    }

    // ══════════════════════════════════════════
    // INPUT
    // ══════════════════════════════════════════

    void OnPlantPressed(InputAction.CallbackContext ctx)
    {
        switch (estado)
        {
            case EstadoSiembra.Inactivo:
                AbrirMenuSemillas();
                break;
            case EstadoSiembra.MenuAbierto:
                CerrarTodo();
                break;
            case EstadoSiembra.Sembrando:
                AbrirMenuSemillas();
                break;
        }
    }

    // ══════════════════════════════════════════
    // ESTADOS
    // ══════════════════════════════════════════

    void AbrirMenuSemillas()
    {
        estado = EstadoSiembra.MenuAbierto;

        // Pausar movimiento, desactivar animación siembra
        if (movimiento != null)
        {
            movimiento.SetMovimientoActivo(false);
            movimiento.SetModoSiembra(false);
        }

        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(true);
        if (panelSembrando    != null) panelSembrando.SetActive(false);
    }

    void EntrarModoSiembra()
    {
        estado = EstadoSiembra.Sembrando;

        // Activar animación de siembra en Beatriz
        if (movimiento != null)
            movimiento.SetModoSiembra(true);

        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(false);
        if (panelSembrando    != null) panelSembrando.SetActive(true);
    }

    void CerrarTodo()
    {
        estado = EstadoSiembra.Inactivo;

        // Reanudar movimiento, desactivar animación siembra
        if (movimiento != null)
        {
            movimiento.SetMovimientoActivo(true);
            movimiento.SetModoSiembra(false);
        }

        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(false);
        if (panelSembrando    != null) panelSembrando.SetActive(false);
    }

    // ══════════════════════════════════════════
    // SELECCIÓN DE SEMILLA
    // ══════════════════════════════════════════

    public void SeleccionarSemilla(int indice)
    {
        semillaSeleccionada = (TypeSeed)indice;

        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("No tienes semillas de tipo " + semillaSeleccionada);
            return;
        }

        Debug.Log("Semilla seleccionada: " + semillaSeleccionada);
        EntrarModoSiembra();
    }

    public void SeleccionarComun()  => SeleccionarSemilla(0);
    public void SeleccionarRed()    => SeleccionarSemilla(1);
    public void SeleccionarGreen()  => SeleccionarSemilla(2);
    public void SeleccionarBlue()   => SeleccionarSemilla(3);
    public void SeleccionarGolden() => SeleccionarSemilla(4);

    // ══════════════════════════════════════════
    // PLANTAR
    // ══════════════════════════════════════════

    void Plantar()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D existente = Physics2D.OverlapCircle(mousePos, radioChequeo);
        if (existente != null && existente.CompareTag("Plant"))
        {
            Debug.Log("Ya hay una planta aquí");
            return;
        }

        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("No tienes semillas de tipo " + semillaSeleccionada);
            CerrarTodo();
            return;
        }

        int manaCosto = ObtenerCostoMana(semillaSeleccionada);
        if (manaSystem != null && !manaSystem.SpendMana(manaCosto))
        {
            Debug.Log("No hay suficiente mana");
            return;
        }

        GameObject prefab = ObtenerPrefab(semillaSeleccionada);
        if (prefab != null)
        {
            Instantiate(prefab, mousePos, Quaternion.identity);
            contenedor.UsarSeed(semillaSeleccionada);
            Debug.Log("Semilla plantada: " + semillaSeleccionada);
        }

        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("Se acabaron las semillas de " + semillaSeleccionada);
            CerrarTodo();
        }
    }

    // ══════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════

    int ObtenerCostoMana(TypeSeed tipo)
    {
        switch (tipo)
        {
            case TypeSeed.Comun:  return 5;
            case TypeSeed.Red:    return 8;
            case TypeSeed.Green:  return 10;
            case TypeSeed.Blue:   return 15;
            case TypeSeed.Golden: return 25;
        }
        return 0;
    }

    GameObject ObtenerPrefab(TypeSeed tipo)
    {
        switch (tipo)
        {
            case TypeSeed.Comun:  return plantaComun;
            case TypeSeed.Red:    return plantaRed;
            case TypeSeed.Green:  return plantaGreen;
            case TypeSeed.Blue:   return plantaBlue;
            case TypeSeed.Golden: return plantaGolden;
        }
        return null;
    }
}
