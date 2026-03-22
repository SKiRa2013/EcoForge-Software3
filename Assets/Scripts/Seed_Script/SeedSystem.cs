using UnityEngine;
using UnityEngine.InputSystem;

// ══════════════════════════════════════════════════════════
// Estados del sistema de siembra
// ══════════════════════════════════════════════════════════
public enum EstadoSiembra
{
    Inactivo,       // Juego normal
    MenuAbierto,    // Menú de selección de semilla abierto
    Sembrando       // Semilla seleccionada, esperando clic para sembrar
}

public class SeedSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BeatrizMovement movimiento;
    [SerializeField] private ContenedorSeedUI contenedor;
    [SerializeField] private ManaSystem manaSystem;

    [Header("UI")]
    [SerializeField] private GameObject panelMenuSemillas;
    [SerializeField] private GameObject panelSembrando;    // indicador visual "haz clic para sembrar"
    [SerializeField] private GameObject cursorSiembra;     // opcional: cursor especial al sembrar

    [Header("Prefabs de plantas")]
    [SerializeField] private GameObject plantaComun;
    [SerializeField] private GameObject plantaRed;
    [SerializeField] private GameObject plantaGreen;
    [SerializeField] private GameObject plantaBlue;
    [SerializeField] private GameObject plantaGolden;

    [Header("Control de espacio")]
    [SerializeField] private float radioChequeo = 0.4f;

    // ── Estado ──
    EstadoSiembra estado = EstadoSiembra.Inactivo;
    TypeSeed semillaSeleccionada = TypeSeed.Comun;

    // ── Input ──
    PlayerInput playerInput;
    InputAction plantAction;

    void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
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
                // ESC cierra el menú
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    CerrarTodo();
                break;

            case EstadoSiembra.Sembrando:
                // Clic izquierdo → sembrar
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    Plantar();

                // ESC o Q → cancelar siembra y volver al menú
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    CerrarTodo();
                break;
        }
    }

    // ══════════════════════════════════════════
    // INPUT — tecla Plant (Q)
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
                // Volver al menú si presiona Q de nuevo
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

        // Pausar movimiento del personaje
        if (movimiento != null)
            movimiento.SetMovimientoActivo(false);

        // Mostrar menú
        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(true);
        if (panelSembrando    != null) panelSembrando.SetActive(false);
        if (cursorSiembra     != null) cursorSiembra.SetActive(false);
    }

    void EntrarModoSiembra()
    {
        estado = EstadoSiembra.Sembrando;

        // Ocultar menú pero mantener movimiento pausado
        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(false);
        if (panelSembrando    != null) panelSembrando.SetActive(true);
        if (cursorSiembra     != null) cursorSiembra.SetActive(true);
    }

    void CerrarTodo()
    {
        estado = EstadoSiembra.Inactivo;

        // Reanudar movimiento
        if (movimiento != null)
            movimiento.SetMovimientoActivo(true);

        // Ocultar todo
        if (panelMenuSemillas != null) panelMenuSemillas.SetActive(false);
        if (panelSembrando    != null) panelSembrando.SetActive(false);
        if (cursorSiembra     != null) cursorSiembra.SetActive(false);
    }

    // ══════════════════════════════════════════
    // SELECCIÓN DE SEMILLA (llamado desde botones UI)
    // ══════════════════════════════════════════

    public void SeleccionarSemilla(int indice)
    {
        semillaSeleccionada = (TypeSeed)indice;

        // Verificar si tiene semillas de ese tipo
        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("No tienes semillas de tipo " + semillaSeleccionada);
            return;
        }

        Debug.Log("Semilla seleccionada: " + semillaSeleccionada);

        // Pasar a modo siembra
        EntrarModoSiembra();
    }

    // Métodos individuales para conectar desde botones UI
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

        // Verificar si hay planta en ese lugar
        Collider2D existente = Physics2D.OverlapCircle(mousePos, radioChequeo);
        if (existente != null && existente.CompareTag("Plant"))
        {
            Debug.Log("Ya hay una planta aquí");
            return;
        }

        // Verificar semillas
        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("No tienes semillas de tipo " + semillaSeleccionada);
            CerrarTodo();
            return;
        }

        // Verificar mana
        int manaCosto = ObtenerCostoMana(semillaSeleccionada);
        if (manaSystem != null && !manaSystem.SpendMana(manaCosto))
        {
            Debug.Log("No hay suficiente mana");
            return;
        }

        // Instanciar planta
        GameObject prefab = ObtenerPrefab(semillaSeleccionada);
        if (prefab != null)
        {
            Instantiate(prefab, mousePos, Quaternion.identity);
            contenedor.UsarSeed(semillaSeleccionada);
            Debug.Log("Semilla plantada: " + semillaSeleccionada);
        }

        // Si no quedan semillas de ese tipo → cerrar
        if (contenedor.ObtenerCantidad(semillaSeleccionada) <= 0)
        {
            Debug.Log("Se acabaron las semillas de " + semillaSeleccionada);
            CerrarTodo();
        }
        // Si quedan semillas → seguir en modo siembra para plantar otra
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