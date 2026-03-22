using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class ControlesManager : MonoBehaviour
{
    [System.Serializable]
    public class ControlEntry
    {
        public string accion;
        public string actionName;
        public Button boton;
        public TextMeshProUGUI txtTecla;
    }

    [Header("Asset de acciones (arrastra el .inputactions)")]
    public InputActionAsset actionAsset;

    [Header("Controles")]
    public List<ControlEntry> controles = new List<ControlEntry>();

    [Header("Panel cambiar tecla")]
    public GameObject panelCambiarTecla;
    public TextMeshProUGUI txtReceptorTecla;

    [Header("Botones")]
    public Button botonRestablecer;

    [Header("Popup cambios pendientes")]
    public GameObject panelConfirmacion;
    public Button btnConfirmarAplicar;
    public Button btnConfirmarCancelar;

    [Header("Popup conflicto")]
    public GameObject panelConflicto;
    public TextMeshProUGUI txtConflicto;
    public Button btnConflictoReemplazar;
    public Button btnConflictoCancelar;

    // ── Estado interno ──
    Dictionary<string, string> pathsPendientes = new Dictionary<string, string>();
    Dictionary<string, string> pathsAplicados  = new Dictionary<string, string>();

    bool hayCambiosPendientes = false;

    ControlEntry entradaEscuchando = null;
    ControlEntry conflictoEntry    = null;
    string       pathConflicto     = "";

    InputActionRebindingExtensions.RebindingOperation operacionRebind;

    void Start()
    {
        if (actionAsset == null)
        {
            Debug.LogError("Asigna el InputActionAsset en el Inspector");
            return;
        }

        // Cargar bindings guardados
        foreach (var entrada in controles)
        {
            var action = actionAsset.FindAction(entrada.actionName, true);

            if (action == null)
            {
                Debug.LogError("No se encontró la acción: " + entrada.actionName);
                continue;
            }

            // Cargar override guardado
            string guardado = PlayerPrefs.GetString("binding_" + entrada.actionName, "");
            if (!string.IsNullOrEmpty(guardado))
                action.ApplyBindingOverride(0, guardado);

            string path = !string.IsNullOrEmpty(action.bindings[0].overridePath)
                ? action.bindings[0].overridePath
                : action.bindings[0].path;

            pathsAplicados[entrada.actionName]  = path;
            pathsPendientes[entrada.actionName] = path;

            ActualizarTextoBoton(entrada, path);

            ControlEntry entradaLocal = entrada;
            entrada.boton.onClick.AddListener(() => IniciarRebind(entradaLocal));
        }

        // Ocultar paneles
        if (panelCambiarTecla != null) panelCambiarTecla.SetActive(false);
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        if (panelConflicto    != null) panelConflicto.SetActive(false);

        // Popup confirmación
        if (btnConfirmarAplicar  != null) btnConfirmarAplicar.onClick.AddListener(PopupAplicar);
        if (btnConfirmarCancelar != null) btnConfirmarCancelar.onClick.AddListener(PopupCancelar);

        // Popup conflicto
        if (btnConflictoReemplazar != null) btnConflictoReemplazar.onClick.AddListener(ReemplazarConflicto);
        if (btnConflictoCancelar   != null) btnConflictoCancelar.onClick.AddListener(CancelarConflicto);
    }

    // ══════════════════════════════════════════
    // REBIND
    // ══════════════════════════════════════════

    void IniciarRebind(ControlEntry entrada)
    {
        entradaEscuchando = entrada;

        if (panelCambiarTecla != null)
        {
            panelCambiarTecla.SetActive(true);
            if (txtReceptorTecla != null)
                txtReceptorTecla.text = "Presiona una tecla para\n[ " + entrada.accion + " ]\n\nESC para cancelar";
        }

        var action = actionAsset.FindAction(entrada.actionName, true);
        action.Disable();

        operacionRebind = action
            .PerformInteractiveRebinding(0)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op => OnRebindComplete(entrada, op))
            .OnCancel(op   => OnRebindCanceled(op))
            .Start();
    }

    void OnRebindComplete(ControlEntry entrada, InputActionRebindingExtensions.RebindingOperation op)
    {
        string nuevaPath = op.action.bindings[0].effectivePath;
        op.Dispose();

        actionAsset.FindAction(entrada.actionName, true).Enable();

        if (panelCambiarTecla != null)
            panelCambiarTecla.SetActive(false);

        ControlEntry conflicto = BuscarConflicto(nuevaPath, entrada);

        if (conflicto != null)
        {
            conflictoEntry = conflicto;
            pathConflicto  = nuevaPath;
            MostrarConflicto(nuevaPath, conflicto);
            return;
        }

        GuardarPendiente(entrada, nuevaPath);
    }

    void OnRebindCanceled(InputActionRebindingExtensions.RebindingOperation op)
    {
        op.Dispose();

        if (entradaEscuchando != null)
        {
            string path = pathsPendientes[entradaEscuchando.actionName];
            var action  = actionAsset.FindAction(entradaEscuchando.actionName, true);
            action.ApplyBindingOverride(0, path);
            action.Enable();
            entradaEscuchando = null;
        }

        if (panelCambiarTecla != null)
            panelCambiarTecla.SetActive(false);
    }

    // ══════════════════════════════════════════
    // CONFLICTO
    // ══════════════════════════════════════════

    ControlEntry BuscarConflicto(string path, ControlEntry ignorar)
    {
        foreach (var entrada in controles)
        {
            if (entrada == ignorar) continue;
            if (pathsPendientes[entrada.actionName] == path)
                return entrada;
        }
        return null;
    }

    void MostrarConflicto(string path, ControlEntry conflicto)
    {
        string tecla = InputControlPath.ToHumanReadableString(
            path, InputControlPath.HumanReadableStringOptions.OmitDevice);

        if (panelConflicto != null) panelConflicto.SetActive(true);

        if (txtConflicto != null)
            txtConflicto.text = "[ " + tecla + " ] ya está asignada a\n[ "
                                + conflicto.accion + " ]\n\n¿Reemplazar?";
    }

    void ReemplazarConflicto()
    {
        var actionConflicto = actionAsset.FindAction(conflictoEntry.actionName, true);
        actionConflicto.RemoveAllBindingOverrides();
        string defaultPath = actionConflicto.bindings[0].path;
        pathsPendientes[conflictoEntry.actionName] = defaultPath;
        ActualizarTextoBoton(conflictoEntry, defaultPath);

        GuardarPendiente(entradaEscuchando, pathConflicto);

        conflictoEntry    = null;
        pathConflicto     = "";
        entradaEscuchando = null;

        if (panelConflicto != null) panelConflicto.SetActive(false);
    }

    void CancelarConflicto()
    {
        if (entradaEscuchando != null)
        {
            string path = pathsPendientes[entradaEscuchando.actionName];
            actionAsset.FindAction(entradaEscuchando.actionName, true).ApplyBindingOverride(0, path);
            entradaEscuchando = null;
        }

        conflictoEntry = null;
        pathConflicto  = "";

        if (panelConflicto != null) panelConflicto.SetActive(false);
    }

    // ══════════════════════════════════════════
    // GUARDAR PENDIENTE
    // ══════════════════════════════════════════

    void GuardarPendiente(ControlEntry entrada, string path)
    {
        actionAsset.FindAction(entrada.actionName, true).ApplyBindingOverride(0, path);
        pathsPendientes[entrada.actionName] = path;
        ActualizarTextoBoton(entrada, path);
        VerificarCambios();
    }

    // ══════════════════════════════════════════
    // APLICAR
    // ══════════════════════════════════════════

    public void AplicarCambios()
    {
        foreach (var entrada in controles)
        {
            string path = pathsPendientes[entrada.actionName];
            pathsAplicados[entrada.actionName] = path;
            PlayerPrefs.SetString("binding_" + entrada.actionName, path);
        }

        PlayerPrefs.Save();
        hayCambiosPendientes = false;

        Debug.Log("Controles guardados");
    }

    // ══════════════════════════════════════════
    // CANCELAR
    // ══════════════════════════════════════════

    public void CancelarCambios()
    {
        foreach (var entrada in controles)
        {
            string path = pathsAplicados[entrada.actionName];
            pathsPendientes[entrada.actionName] = path;
            actionAsset.FindAction(entrada.actionName, true).ApplyBindingOverride(0, path);
            ActualizarTextoBoton(entrada, path);
        }

        hayCambiosPendientes = false;
    }

    // ══════════════════════════════════════════
    // RESTABLECER DEFAULTS
    // ══════════════════════════════════════════

    public void RestaurarDefaults()
    {
        foreach (var entrada in controles)
        {
            var action = actionAsset.FindAction(entrada.actionName, true);
            action.RemoveAllBindingOverrides();
            string path = action.bindings[0].path;
            pathsPendientes[entrada.actionName] = path;
            ActualizarTextoBoton(entrada, path);
        }

        VerificarCambios();
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
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        VolverOpciones();
    }

    void PopupCancelar()
    {
        CancelarCambios();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        VolverOpciones();
    }

    void VolverOpciones()
    {
        FindFirstObjectByType<OpcionesManager>()?.MostrarGeneral();
    }

    // ══════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════

    void ActualizarTextoBoton(ControlEntry entrada, string path)
    {
        if (entrada.txtTecla == null) return;

        string nombre = InputControlPath.ToHumanReadableString(
            path, InputControlPath.HumanReadableStringOptions.OmitDevice);

        entrada.txtTecla.text = nombre;
    }

    void VerificarCambios()
    {
        hayCambiosPendientes = false;

        foreach (var entrada in controles)
        {
            if (pathsPendientes[entrada.actionName] != pathsAplicados[entrada.actionName])
            {
                hayCambiosPendientes = true;
                break;
            }
        }
    }
}
