using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

public class ManagerIdioma : MonoBehaviour
{
    [Header("Controles")]
    public TMP_Dropdown dropdownIdioma;

    [Header("Botones")]
    public Button botonAplicar;
    public Button botonCancelar;

    [Header("Popup")]
    public GameObject panelConfirmacion;
    public Button btnConfirmarAplicar;
    public Button btnConfirmarCancelar;

    int idiomaAplicado;
    int idiomaPendiente;
    bool hayCambiosPendientes = false;
    bool inicializado = false;

    void Start()
    {
        idiomaAplicado  = PlayerPrefs.GetInt("idioma", 0);
        idiomaPendiente = idiomaAplicado;

        dropdownIdioma.onValueChanged.RemoveAllListeners();
        dropdownIdioma.ClearOptions();
        dropdownIdioma.AddOptions(new System.Collections.Generic.List<string> { "Español", "English" });
        dropdownIdioma.value = idiomaAplicado;
        dropdownIdioma.RefreshShownValue();
        dropdownIdioma.onValueChanged.AddListener(CambiarIdioma);

        inicializado = true;

        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);

        if (btnConfirmarAplicar  != null) btnConfirmarAplicar.onClick.AddListener(PopupAplicar);
        if (btnConfirmarCancelar != null) btnConfirmarCancelar.onClick.AddListener(PopupCancelar);

        ActualizarBotones();
    }

    public void CambiarIdioma(int indice)
    {
        if (!inicializado) return;

        idiomaPendiente = indice;
        VerificarCambios();
    }

    public void AplicarCambios()
    {
        StartCoroutine(AplicarIdiomaCoroutine(idiomaPendiente));
    }

    System.Collections.IEnumerator AplicarIdiomaCoroutine(int indice)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locale = LocalizationSettings.AvailableLocales.Locales[indice];
        LocalizationSettings.SelectedLocale = locale;

        PlayerPrefs.SetInt("idioma", indice);
        PlayerPrefs.Save();

        idiomaAplicado       = indice;
        hayCambiosPendientes = false;

        ActualizarBotones();
    }

    public void CancelarCambios()
    {
        idiomaPendiente = idiomaAplicado;

        dropdownIdioma.onValueChanged.RemoveListener(CambiarIdioma);
        dropdownIdioma.value = idiomaAplicado;
        dropdownIdioma.onValueChanged.AddListener(CambiarIdioma);

        hayCambiosPendientes = false;
        ActualizarBotones();
    }

    public void Regresar()
    {
        if (hayCambiosPendientes)
        {
            if (panelConfirmacion != null)
                panelConfirmacion.SetActive(true);
        }
        else
        {
            VolverOpciones();
        }
    }

    void PopupAplicar()
    {
        StartCoroutine(PopupAplicarCoroutine());
    }

    System.Collections.IEnumerator PopupAplicarCoroutine()
    {
        yield return AplicarIdiomaCoroutine(idiomaPendiente);
        CerrarPopup();
        VolverOpciones();
    }

    void PopupCancelar()
    {
        CancelarCambios();
        CerrarPopup();
        VolverOpciones();
    }

    void CerrarPopup()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
    }

    void VolverOpciones()
    {
        FindFirstObjectByType<MenuManager>()?.VolverOpciones();
    }

    void VerificarCambios()
    {
        hayCambiosPendientes = idiomaPendiente != idiomaAplicado;
        ActualizarBotones();
    }

    void ActualizarBotones()
    {
        if (botonAplicar  != null) botonAplicar.interactable  = hayCambiosPendientes;
        if (botonCancelar != null) botonCancelar.interactable = hayCambiosPendientes;
    }
}
