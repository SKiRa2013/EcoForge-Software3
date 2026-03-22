using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Índice del slot")]
    public int slotIndex;

    [Header("Textos")]
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtFecha;
    public TextMeshProUGUI txtProgreso;

    [Header("Botones")]
    public Button btnGuardar;
    public Button btnCargar;
    public Button btnBorrar;
    public Button btnRenombrar;

    [Header("Popup confirmar borrar")]
    public GameObject panelConfirmarBorrar;
    public Button btnConfirmarBorrar;
    public Button btnCancelarBorrar;

    [Header("Popup renombrar")]
    public GameObject panelRenombrar;
    public TMP_InputField inputNuevoNombre;
    public TextMeshProUGUI txtErrorRenombrar;
    public Button btnConfirmarRenombrar;
    public Button btnCancelarRenombrar;

    SaveGamePanel panelPadre;

    void Start()
    {
        panelPadre = GetComponentInParent<SaveGamePanel>();

        if (panelConfirmarBorrar != null) panelConfirmarBorrar.SetActive(false);
        if (panelRenombrar       != null) panelRenombrar.SetActive(false);

        if (btnConfirmarBorrar    != null) btnConfirmarBorrar.onClick.AddListener(ConfirmarBorrar);
        if (btnCancelarBorrar     != null) btnCancelarBorrar.onClick.AddListener(CancelarBorrar);
        if (btnConfirmarRenombrar != null) btnConfirmarRenombrar.onClick.AddListener(ConfirmarRenombrar);
        if (btnCancelarRenombrar  != null) btnCancelarRenombrar.onClick.AddListener(CancelarRenombrar);
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
                default:                       return key;
            }
        }
    }

    void MostrarError(string key)
    {
        if (txtErrorRenombrar    != null) txtErrorRenombrar.text = ObtenerTexto(key);
        if (btnConfirmarRenombrar != null) btnConfirmarRenombrar.interactable = false;
    }

    void LimpiarError()
    {
        if (txtErrorRenombrar != null) txtErrorRenombrar.text = "";
    }

    // ── SLOT VACÍO ──
    public void SetEmpty()
    {
        if (txtNombre   != null) txtNombre.text  = "Vacío";
        if (txtFecha    != null) txtFecha.text    = "-";
        if (txtProgreso != null) txtProgreso.text = "-";

        if (btnCargar    != null) btnCargar.interactable    = false;
        if (btnBorrar    != null) btnBorrar.interactable    = false;
        if (btnRenombrar != null) btnRenombrar.interactable = false;
    }

    // ── SLOT CON DATOS ──
    public void SetData(string nombre, string fecha, string progreso)
    {
        if (txtNombre   != null) txtNombre.text  = nombre;
        if (txtFecha    != null) txtFecha.text    = fecha;
        if (txtProgreso != null) txtProgreso.text = progreso;

        if (btnCargar    != null) btnCargar.interactable    = true;
        if (btnBorrar    != null) btnBorrar.interactable    = true;
        if (btnRenombrar != null) btnRenombrar.interactable = true;
    }

    // ── GUARDAR ──
    public void Guardar()
    {
        GameManager.Instance.GuardarPartida(slotIndex);
        if (panelPadre != null) panelPadre.ActualizarSlots();
    }

    // ── CARGAR ──
    public void Cargar()
    {
        PlayerPrefs.SetInt("slotActivo", slotIndex);
        PlayerPrefs.Save();
        GameManager.Instance.CargarPartida(slotIndex);
    }

    // ── BORRAR ──
    public void Borrar()
    {
        if (panelConfirmarBorrar != null)
            panelConfirmarBorrar.SetActive(true);
    }

    void ConfirmarBorrar()
    {
        SaveSystem.BorrarSave(slotIndex);
        if (panelConfirmarBorrar != null) panelConfirmarBorrar.SetActive(false);
        if (panelPadre           != null) panelPadre.ActualizarSlots();
    }

    void CancelarBorrar()
    {
        if (panelConfirmarBorrar != null) panelConfirmarBorrar.SetActive(false);
    }

    // ── RENOMBRAR ──
    public void Renombrar()
    {
        if (inputNuevoNombre != null)
        {
            var data = SaveSystem.CargarInfo(slotIndex);
            inputNuevoNombre.text = data != null ? data.nombre : "";
            inputNuevoNombre.onValueChanged.RemoveAllListeners();
            inputNuevoNombre.onValueChanged.AddListener(OnNombreCambiado);
        }

        LimpiarError();
        if (btnConfirmarRenombrar != null) btnConfirmarRenombrar.interactable = false;
        if (panelRenombrar        != null) panelRenombrar.SetActive(true);
        if (inputNuevoNombre      != null) inputNuevoNombre.Select();
    }

    void OnNombreCambiado(string valor)
    {
        LimpiarError();

        string nombre = valor.Trim();

        if (string.IsNullOrEmpty(nombre))     { MostrarError("error_nombre_vacio");     return; }
        if (nombre.Length < 3)                { MostrarError("error_nombre_corto");     return; }
        if (nombre.Length > 20)               { MostrarError("error_nombre_largo");     return; }
        if (SaveSystem.NombreExiste(nombre, slotIndex)) { MostrarError("error_nombre_duplicado"); return; }

        if (btnConfirmarRenombrar != null) btnConfirmarRenombrar.interactable = true;
    }

    void ConfirmarRenombrar()
    {
        string nuevoNombre = inputNuevoNombre.text.Trim();

        if (SaveSystem.NombreExiste(nuevoNombre, slotIndex))
        {
            MostrarError("error_nombre_duplicado");
            return;
        }

        SaveSystem.RenombrarPartida(slotIndex, nuevoNombre);
        if (panelRenombrar != null) panelRenombrar.SetActive(false);
        if (panelPadre     != null) panelPadre.ActualizarSlots();
    }

    void CancelarRenombrar()
    {
        if (panelRenombrar != null) panelRenombrar.SetActive(false);
    }
}