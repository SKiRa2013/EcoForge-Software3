using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer (opcional)")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider sliderVolumenGeneral;
    public Slider sliderVolumenMusica;
    public Slider sliderVolumenEfectos;

    [Header("Dropdown")]
    public TMP_Dropdown dropdownDispositivoSalida;

    [Header("Botones")]
    public Button botonAplicar;
    public Button botonCancelar;

    [Header("Popup cambios pendientes")]
    public GameObject panelConfirmacion;
    public Button btnConfirmarAplicar;
    public Button btnConfirmarCancelar;

    // ── Valores aplicados ──
    float volGeneralAplicado;
    float volMusicaAplicado;
    float volEfectosAplicado;
    int   dispositivoAplicado;

    // ── Valores pendientes ──
    float volGeneralPendiente;
    float volMusicaPendiente;
    float volEfectosPendiente;
    int   dispositivoPendiente;

    bool hayCambiosPendientes = false;
    bool inicializado         = false;

    void Start()
    {
        // Cargar valores guardados
        volGeneralAplicado  = PlayerPrefs.GetFloat("volGeneral",  1f);
        volMusicaAplicado   = PlayerPrefs.GetFloat("volMusica",   1f);
        volEfectosAplicado  = PlayerPrefs.GetFloat("volEfectos",  1f);
        dispositivoAplicado = PlayerPrefs.GetInt("dispositivo",   0);

        // Inicializar pendientes
        volGeneralPendiente  = volGeneralAplicado;
        volMusicaPendiente   = volMusicaAplicado;
        volEfectosPendiente  = volEfectosAplicado;
        dispositivoPendiente = dispositivoAplicado;

        // Aplicar al listener al iniciar
        AudioListener.volume = volGeneralAplicado;

        // Aplicar al mixer si está asignado
        AplicarAlMixer(volGeneralAplicado, volMusicaAplicado, volEfectosAplicado);

        // Configurar sliders sin disparar eventos
        sliderVolumenGeneral.onValueChanged.RemoveAllListeners();
        sliderVolumenMusica.onValueChanged.RemoveAllListeners();
        sliderVolumenEfectos.onValueChanged.RemoveAllListeners();

        sliderVolumenGeneral.value  = volGeneralAplicado;
        sliderVolumenMusica.value   = volMusicaAplicado;
        sliderVolumenEfectos.value  = volEfectosAplicado;

        sliderVolumenGeneral.onValueChanged.AddListener(OnVolumenGeneralChanged);
        sliderVolumenMusica.onValueChanged.AddListener(OnVolumenMusicaChanged);
        sliderVolumenEfectos.onValueChanged.AddListener(OnVolumenEfectosChanged);

        // Configurar dropdown
        ConfigurarDropdown();

        // Popup oculto
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);

        if (btnConfirmarAplicar  != null) btnConfirmarAplicar.onClick.AddListener(PopupAplicar);
        if (btnConfirmarCancelar != null) btnConfirmarCancelar.onClick.AddListener(PopupCancelar);

        inicializado = true;
        ActualizarBotones();
    }

    // ══════════════════════════════════════════
    // DROPDOWN DISPOSITIVO
    // ══════════════════════════════════════════

    void ConfigurarDropdown()
    {
        dropdownDispositivoSalida.onValueChanged.RemoveAllListeners();
        dropdownDispositivoSalida.ClearOptions();

        var opciones = new List<string>();

        // Obtener dispositivos de audio disponibles
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        opciones.Add("Dispositivo por defecto");
        opciones.Add("Auriculares");
        opciones.Add("Altavoces");
        #else
        opciones.Add("Dispositivo por defecto");
        opciones.Add("Auriculares");
        opciones.Add("Altavoces");
        #endif

        dropdownDispositivoSalida.AddOptions(opciones);
        dropdownDispositivoSalida.value = dispositivoAplicado;
        dropdownDispositivoSalida.RefreshShownValue();

        dropdownDispositivoSalida.onValueChanged.AddListener(OnDispositivoChanged);
    }

    // ══════════════════════════════════════════
    // CAMBIOS PENDIENTES
    // ══════════════════════════════════════════

    void OnVolumenGeneralChanged(float valor)
    {
        if (!inicializado) return;
        volGeneralPendiente = valor;
        VerificarCambios();
    }

    void OnVolumenMusicaChanged(float valor)
    {
        if (!inicializado) return;
        volMusicaPendiente = valor;
        VerificarCambios();
    }

    void OnVolumenEfectosChanged(float valor)
    {
        if (!inicializado) return;
        volEfectosPendiente = valor;
        VerificarCambios();
    }

    void OnDispositivoChanged(int indice)
    {
        if (!inicializado) return;
        dispositivoPendiente = indice;
        VerificarCambios();
    }

    // ══════════════════════════════════════════
    // APLICAR
    // ══════════════════════════════════════════

    public void AplicarCambios()
    {
        AudioListener.volume = volGeneralPendiente;

        AplicarAlMixer(volGeneralPendiente, volMusicaPendiente, volEfectosPendiente);

        PlayerPrefs.SetFloat("volGeneral",  volGeneralPendiente);
        PlayerPrefs.SetFloat("volMusica",   volMusicaPendiente);
        PlayerPrefs.SetFloat("volEfectos",  volEfectosPendiente);
        PlayerPrefs.SetInt("dispositivo",   dispositivoPendiente);
        PlayerPrefs.Save();

        volGeneralAplicado  = volGeneralPendiente;
        volMusicaAplicado   = volMusicaPendiente;
        volEfectosAplicado  = volEfectosPendiente;
        dispositivoAplicado = dispositivoPendiente;

        hayCambiosPendientes = false;
        ActualizarBotones();

        Debug.Log("Audio aplicado");
    }

    void AplicarAlMixer(float general, float musica, float efectos)
    {
        if (audioMixer == null) return;

        // Convertir de 0-1 a dB (-80 a 0)
        audioMixer.SetFloat("VolGeneral", general  > 0 ? Mathf.Log10(general)  * 20 : -80f);
        audioMixer.SetFloat("VolMusica",  musica   > 0 ? Mathf.Log10(musica)   * 20 : -80f);
        audioMixer.SetFloat("VolEfectos", efectos  > 0 ? Mathf.Log10(efectos)  * 20 : -80f);
    }

    // ══════════════════════════════════════════
    // CANCELAR
    // ══════════════════════════════════════════

    public void CancelarCambios()
    {
        volGeneralPendiente  = volGeneralAplicado;
        volMusicaPendiente   = volMusicaAplicado;
        volEfectosPendiente  = volEfectosAplicado;
        dispositivoPendiente = dispositivoAplicado;

        // Revertir sliders sin disparar eventos
        sliderVolumenGeneral.onValueChanged.RemoveListener(OnVolumenGeneralChanged);
        sliderVolumenMusica.onValueChanged.RemoveListener(OnVolumenMusicaChanged);
        sliderVolumenEfectos.onValueChanged.RemoveListener(OnVolumenEfectosChanged);
        dropdownDispositivoSalida.onValueChanged.RemoveListener(OnDispositivoChanged);

        sliderVolumenGeneral.value  = volGeneralAplicado;
        sliderVolumenMusica.value   = volMusicaAplicado;
        sliderVolumenEfectos.value  = volEfectosAplicado;
        dropdownDispositivoSalida.value = dispositivoAplicado;

        sliderVolumenGeneral.onValueChanged.AddListener(OnVolumenGeneralChanged);
        sliderVolumenMusica.onValueChanged.AddListener(OnVolumenMusicaChanged);
        sliderVolumenEfectos.onValueChanged.AddListener(OnVolumenEfectosChanged);
        dropdownDispositivoSalida.onValueChanged.AddListener(OnDispositivoChanged);

        hayCambiosPendientes = false;
        ActualizarBotones();
    }

    // ══════════════════════════════════════════
    // REGRESAR
    // ══════════════════════════════════════════

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
        AplicarCambios();
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

    // ══════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════

    void VerificarCambios()
    {
        hayCambiosPendientes =
            !Mathf.Approximately(volGeneralPendiente,  volGeneralAplicado)  ||
            !Mathf.Approximately(volMusicaPendiente,   volMusicaAplicado)   ||
            !Mathf.Approximately(volEfectosPendiente,  volEfectosAplicado)  ||
            dispositivoPendiente != dispositivoAplicado;

        ActualizarBotones();
    }

    void ActualizarBotones()
    {
        if (botonAplicar  != null) botonAplicar.interactable  = hayCambiosPendientes;
        if (botonCancelar != null) botonCancelar.interactable = hayCambiosPendientes;
    }
}
