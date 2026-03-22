using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    [Header("Controles de configuración")]
    public TMP_Dropdown dropdownResolucion;
    public TMP_Dropdown dropdownCalidad;
    public Toggle togglePantallaCompleta;

    [Header("Botones")]
    public Button botonAplicar;
    public Button botonCancelar;
    public Button botonRegresar;

    [Header("Popup de cambios pendientes")]
    public GameObject panelConfirmacion;
    public Button btnConfirmarAplicar;
    public Button btnConfirmarCancelar;

    // ── Valores aplicados (los que están activos en el juego) ──
    Resolution[] resoluciones;
    int resolucionAplicada;
    int calidadAplicada;
    bool pantallaCompletaAplicada;

    // ── Valores pendientes (lo que el usuario seleccionó pero no aplicó) ──
    int resolucionPendiente;
    int calidadPendiente;
    bool pantallaCompletaPendiente;

    // ── Flag para saber si hay cambios sin aplicar ──
    bool hayCambiosPendientes = false;

    void Start()
    {
        // ── Resoluciones ──
        resoluciones = Screen.resolutions;
        dropdownResolucion.ClearOptions();

        int resolucionActual = 0;
        var opciones = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
                resolucionActual = i;
        }

        dropdownResolucion.AddOptions(opciones);
        dropdownResolucion.value = resolucionActual;
        dropdownResolucion.RefreshShownValue();

        // ── Calidad ──
        int calidad = PlayerPrefs.GetInt("calidadGraficos", 5);
        dropdownCalidad.value = calidad;
        QualitySettings.SetQualityLevel(calidad);

        // ── Pantalla completa ──
        bool pantallaCompleta = PlayerPrefs.GetInt("pantallaCompleta", 1) == 1;
        togglePantallaCompleta.isOn = pantallaCompleta;
        Screen.fullScreen = pantallaCompleta;

        // ── Guardar valores aplicados ──
        resolucionAplicada    = resolucionActual;
        calidadAplicada       = calidad;
        pantallaCompletaAplicada = pantallaCompleta;

        // ── Pendientes inician igual a los aplicados ──
        resolucionPendiente      = resolucionActual;
        calidadPendiente         = calidad;
        pantallaCompletaPendiente = pantallaCompleta;

        // ── Popup inicia oculto ──
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);

        // ── Conectar botones del popup ──
        if (btnConfirmarAplicar != null)
            btnConfirmarAplicar.onClick.AddListener(PopupAplicar);

        if (btnConfirmarCancelar != null)
            btnConfirmarCancelar.onClick.AddListener(PopupCancelar);

        ActualizarBotones();
    }

    // ══════════════════════════════════════════
    // CAMBIOS PENDIENTES (no aplican todavía)
    // ══════════════════════════════════════════

    public void CambiarResolucion(int indice)
    {
        resolucionPendiente = indice;
        VerificarCambios();
    }

    public void CambiarCalidad(int indice)
    {
        calidadPendiente = indice;
        VerificarCambios();
    }

    public void PantallaCompleta(bool activo)
    {
        pantallaCompletaPendiente = activo;
        VerificarCambios();
    }

    // ══════════════════════════════════════════
    // APLICAR
    // ══════════════════════════════════════════

    public void AplicarCambios()
    {
        // Resolución
        Resolution res = resoluciones[resolucionPendiente];
        Screen.SetResolution(res.width, res.height, pantallaCompletaPendiente);

        // Calidad
        QualitySettings.SetQualityLevel(calidadPendiente);
        PlayerPrefs.SetInt("calidadGraficos", calidadPendiente);

        // Pantalla completa
        Screen.fullScreen = pantallaCompletaPendiente;
        PlayerPrefs.SetInt("pantallaCompleta", pantallaCompletaPendiente ? 1 : 0);

        PlayerPrefs.Save();

        // ── Actualizar valores aplicados ──
        resolucionAplicada       = resolucionPendiente;
        calidadAplicada          = calidadPendiente;
        pantallaCompletaAplicada = pantallaCompletaPendiente;

        hayCambiosPendientes = false;
        ActualizarBotones();

        Debug.Log("Cambios gráficos aplicados");
    }

    // ══════════════════════════════════════════
    // CANCELAR (descarta pendientes, vuelve a aplicados)
    // ══════════════════════════════════════════

    public void CancelarCambios()
    {
        // Revertir pendientes a los valores que están activos
        resolucionPendiente      = resolucionAplicada;
        calidadPendiente         = calidadAplicada;
        pantallaCompletaPendiente = pantallaCompletaAplicada;

        // Revertir la UI visualmente
        dropdownResolucion.value    = resolucionAplicada;
        dropdownCalidad.value       = calidadAplicada;
        togglePantallaCompleta.isOn = pantallaCompletaAplicada;

        hayCambiosPendientes = false;
        ActualizarBotones();

        Debug.Log("Cambios cancelados");
    }

    // ══════════════════════════════════════════
    // REGRESAR (con chequeo de cambios pendientes)
    // ══════════════════════════════════════════

    public void Regresar()
    {
        if (hayCambiosPendientes)
        {
            // Mostrar popup de confirmación
            if (panelConfirmacion != null)
                panelConfirmacion.SetActive(true);
        }
        else
        {
            // No hay cambios, regresar directo
            VolverOpciones();
        }
    }

    // ── Botones del popup ──

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
        // Busca el MenuManager y llama VolverOpciones
        FindFirstObjectByType<MenuManager>()?.VolverOpciones();
    }

    // ══════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════

    void VerificarCambios()
    {
        hayCambiosPendientes =
            resolucionPendiente      != resolucionAplicada       ||
            calidadPendiente         != calidadAplicada          ||
            pantallaCompletaPendiente != pantallaCompletaAplicada;

        ActualizarBotones();
    }

    void ActualizarBotones()
    {
        // Aplicar y Cancelar solo están activos si hay cambios pendientes
        if (botonAplicar  != null) botonAplicar.interactable  = hayCambiosPendientes;
        if (botonCancelar != null) botonCancelar.interactable = hayCambiosPendientes;
    }
}
