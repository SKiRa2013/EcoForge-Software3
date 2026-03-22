using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instancia;

    public GameObject panelPausa;

    bool juegoPausado = false;

    PlayerInput playerInput;
    InputAction pauseAction;

    void Awake()
    {
        // ── Sin DontDestroyOnLoad ──
        // El PauseManager vive solo en la escena Juego
        // Cada vez que se carga la escena se crea uno nuevo limpio
        instancia = this;

        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("No se encontró PlayerInput en PauseManager");
            return;
        }

        pauseAction = playerInput.actions["Pause"];
    }

    void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.Enable();
            pauseAction.performed += OnPausePressed;
        }
    }

    void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePressed;
        }
    }

    void Start()
    {
        // Asegurarse que el juego inicia sin pausa
        juegoPausado = false;
        Time.timeScale = 1f;

        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    void OnPausePressed(InputAction.CallbackContext context)
    {
        // Solo funcionar en la escena del juego
        if (SceneManager.GetActiveScene().name != "Juego") return;

        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }

    public void Pausar()
    {
        if (panelPausa != null)
            panelPausa.SetActive(true);

        Time.timeScale = 0f;
        juegoPausado = true;
    }

    public void Reanudar()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void VolverMenu()
    {
        // Resetear estado antes de salir
        juegoPausado = false;
        Time.timeScale = 1f;

        // Guardar antes de salir
        GameManager.Instance?.GuardarPartidaActiva();

        SceneManager.LoadScene("MenuPrincipal");
    }
}