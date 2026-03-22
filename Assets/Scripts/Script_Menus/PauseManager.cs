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
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("No se encontró PlayerInput en este objeto.");
            return;
        }

        pauseAction = playerInput.actions["Pause"];

        if (pauseAction == null)
        {
            Debug.LogError("No se encontró la acción 'Pause' en InputActions.");
        }
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
            pauseAction.Disable();
        }
    }

    void OnPausePressed(InputAction.CallbackContext context)
    {
        Debug.Log("ESC presionado");

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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
