using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using TMPro;

public class NuevaPartidaPanel : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputNombre;
    public TextMeshProUGUI txtError;
    public Button btnConfirmar;
    public Button btnCancelar;

    int slotSeleccionado = -1;

    void Start()
    {
        if (txtError     != null) txtError.text = "";
        if (btnConfirmar != null) btnConfirmar.interactable = false;
    }

    // ── Texto localizado ──
    string ObtenerTexto(string key)
    {
        try
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString("UITexts", key);
        }
        catch
        {
            switch (key)
            {
                case "error_nombre_vacio":     return "El nombre no puede estar vacío";
                case "error_nombre_corto":     return "Mínimo 3 caracteres";
                case "error_nombre_largo":     return "Máximo 20 caracteres";
                case "error_nombre_duplicado": return "Ya existe una partida con ese nombre";
                case "error_slots_llenos":     return "No hay slots disponibles";
                default:                       return key;
            }
        }
    }

    void MostrarError(string key)
    {
        if (txtError     != null) txtError.text = ObtenerTexto(key);
        if (btnConfirmar != null) btnConfirmar.interactable = false;
    }

    void LimpiarError()
    {
        if (txtError != null) txtError.text = "";
    }

    // ── Llamado desde MenuManager al abrir el panel ──
    public void AbrirNuevaPartida()
    {
        slotSeleccionado = SaveSystem.ObtenerSlotLibre();

        if (inputNombre  != null) inputNombre.text = "";
        if (btnConfirmar != null) btnConfirmar.interactable = false;
        LimpiarError();

        if (slotSeleccionado == -1)
        {
            MostrarError("error_slots_llenos");
            return;
        }

        if (inputNombre != null) inputNombre.Select();

        Debug.Log("NuevaPartidaPanel abierto. Slot disponible: " + slotSeleccionado);
    }

    // ── Validar mientras escribe ──
    public void OnNombreCambiado(string valor)
    {
        LimpiarError();

        string nombre = valor.Trim();

        if (string.IsNullOrEmpty(nombre))   { MostrarError("error_nombre_vacio");     return; }
        if (nombre.Length < 3)              { MostrarError("error_nombre_corto");     return; }
        if (nombre.Length > 20)             { MostrarError("error_nombre_largo");     return; }
        if (SaveSystem.NombreExiste(nombre)){ MostrarError("error_nombre_duplicado"); return; }

        if (btnConfirmar != null) btnConfirmar.interactable = true;
    }

    // ── Confirmar ──
    public void Confirmar()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            MostrarError("error_nombre_vacio");
            return;
        }

        if (SaveSystem.NombreExiste(nombre))
        {
            MostrarError("error_nombre_duplicado");
            return;
        }

        SaveSystem.CrearPartida(slotSeleccionado, nombre);
        PlayerPrefs.SetInt("slotActivo", slotSeleccionado);
        PlayerPrefs.Save();

        Debug.Log("Partida creada: " + nombre + " en slot " + slotSeleccionado);

        SceneManager.LoadScene("Juego");
    }

    // ── Cancelar ── MenuManager maneja cerrar el panel
    public void Cancelar()
    {
        if (inputNombre != null) inputNombre.text = "";
        LimpiarError();

        // Volver al menú principal
        FindFirstObjectByType<MenuManager>()?.VolverMenu();
    }
}
