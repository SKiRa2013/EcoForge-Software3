using UnityEngine;

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
        }
    }

    // ── GUARDAR usando slot activo ──
    public void GuardarPartida(int slot)
    {
        if (player == null)
        {
            Debug.LogWarning("Player no asignado en GameManager");
            return;
        }

        // Conservar el nombre existente si ya hay datos
        string nombre = "Partida " + (slot + 1);
        var dataExistente = SaveSystem.CargarInfo(slot);
        if (dataExistente != null && !string.IsNullOrEmpty(dataExistente.nombre))
            nombre = dataExistente.nombre;

        SaveSystem.GuardarJuego(slot, player, nombre);

        Debug.Log("Partida guardada en slot: " + slot);
    }

    // ── GUARDAR en slot activo automáticamente ──
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

        // Cargar la escena guardada
        UnityEngine.SceneManagement.SceneManager.LoadScene(data.escena);

        Debug.Log("Partida cargada desde slot: " + slot + " | " + data.nombre);
    }
}