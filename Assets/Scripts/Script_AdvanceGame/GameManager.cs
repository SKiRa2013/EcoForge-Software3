using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias")]
    public Transform player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    void OnScenaCargada(Scene escena, LoadSceneMode mode)
    {
        // Buscar al player en la nueva escena
        if (player == null)
        {
            var beatriz = GameObject.FindWithTag("Player");
            if (beatriz != null)
                player = beatriz.transform;
        }

        // Guardar automáticamente al entrar a la escena del juego
        if (escena.name == "Juego")
        {
            // Esperar un frame para que todo esté inicializado
            StartCoroutine(GuardarAlIniciar());
        }
    }

    System.Collections.IEnumerator GuardarAlIniciar()
    {
        yield return null; // esperar un frame

        // Buscar player si no está asignado
        if (player == null)
        {
            var beatriz = GameObject.FindWithTag("Player");
            if (beatriz != null)
                player = beatriz.transform;
        }

        int slot = PlayerPrefs.GetInt("slotActivo", 0);
        GuardarPartida(slot);
    }

    // ── GUARDAR ──
    public void GuardarPartida(int slot)
    {
        if (player == null)
        {
            Debug.LogWarning("Player no asignado en GameManager");
            return;
        }

        // Conservar nombre existente
        string nombre = "Partida " + (slot + 1);
        var dataExistente = SaveSystem.CargarInfo(slot);
        if (dataExistente != null && !string.IsNullOrEmpty(dataExistente.nombre))
            nombre = dataExistente.nombre;

        SaveSystem.GuardarJuego(slot, player, nombre);
        Debug.Log("Partida guardada en slot: " + slot);
    }

    public void GuardarPartidaActiva()
    {
        int slot = PlayerPrefs.GetInt("slotActivo", 0);
        GuardarPartida(slot);
    }

    // ── CARGAR ──
    public void CargarPartida(int slot)
    {
        GameData data = SaveSystem.CargarJuego(slot);

        if (data == null)
        {
            Debug.Log("No hay datos en este slot");
            return;
        }

        PlayerPrefs.SetInt("slotActivo", slot);
        PlayerPrefs.Save();

        if (player != null)
            player.position = new Vector2(data.posX, data.posY);

        SceneManager.LoadScene(data.escena);
        Debug.Log("Partida cargada: " + data.nombre);
    }
}