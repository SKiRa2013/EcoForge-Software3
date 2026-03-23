using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels principales")]
    public GameObject panelMenuPrincipal;
    public GameObject panelPartidas;
    public GameObject panelOpciones;
    public GameObject panelAudio;
    public GameObject panelGraficos;
    public GameObject panelControles;
    public GameObject panelIdioma;
    public GameObject panelCreditos;

    [Header("Subpaneles de partidas")]
    public GameObject panelNuevaPartida;   // hijo de panelPartidas
    public GameObject panelCargarPartida;  // hijo de panelPartidas

    void Start()
    {
        MostrarMenuPrincipal();
    }

    // ══════════════════════════════════════════
    // MENU PRINCIPAL
    // ══════════════════════════════════════════

    public void MostrarMenuPrincipal()
    {
        panelMenuPrincipal.SetActive(true);
        panelPartidas.SetActive(false);
        panelOpciones.SetActive(false);
        panelAudio.SetActive(false);
        panelGraficos.SetActive(false);
        panelControles.SetActive(false);
        panelIdioma.SetActive(false);
        panelCreditos.SetActive(false);

        // Desactivar subpaneles
        if (panelNuevaPartida  != null) panelNuevaPartida.SetActive(false);
        if (panelCargarPartida != null) panelCargarPartida.SetActive(false);
    }

    // ── Botón JUGAR → abre nueva partida ──
    public void IniciarPartida()
    {
        Debug.Log("IniciarPartida llamado");
        Debug.Log("panelMenuPrincipal: " + panelMenuPrincipal);
        Debug.Log("panelPartidas: " + panelPartidas);
        Debug.Log("panelNuevaPartida: " + panelNuevaPartida);

        panelMenuPrincipal.SetActive(false);
        panelPartidas.SetActive(true);
        panelNuevaPartida.SetActive(true);
        panelCargarPartida.SetActive(false);

        var script = panelNuevaPartida.GetComponent<NuevaPartidaPanel>();
        if (script != null)
            script.AbrirNuevaPartida();
        else
            Debug.LogError("No se encontró NuevaPartidaPanel");
    }

    // ── Botón CARGAR PARTIDA → abre cargar partida ──
    public void AbrirPartidas()
    {
        panelMenuPrincipal.SetActive(false);
        panelPartidas.SetActive(true);
        panelNuevaPartida.SetActive(false);
        panelCargarPartida.SetActive(true);
    }

    // ══════════════════════════════════════════
    // OPCIONES
    // ══════════════════════════════════════════

    public void AbrirOpciones()
    {
        panelMenuPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void AbrirAudio()
    {
        panelOpciones.SetActive(false);
        panelAudio.SetActive(true);
    }

    public void AbrirGraficos()
    {
        panelOpciones.SetActive(false);
        panelGraficos.SetActive(true);
    }

    public void AbrirControles()
    {
        panelOpciones.SetActive(false);
        panelControles.SetActive(true);
    }

    public void AbrirIdioma()
    {
        panelOpciones.SetActive(false);
        panelIdioma.SetActive(true);
    }

    // ══════════════════════════════════════════
    // CRÉDITOS
    // ══════════════════════════════════════════

    public void AbrirCreditos()
    {
        panelMenuPrincipal.SetActive(false);
        panelCreditos.SetActive(true);
    }

    // ══════════════════════════════════════════
    // BOTONES VOLVER
    // ══════════════════════════════════════════

    public void VolverOpciones()
    {
        panelAudio.SetActive(false);
        panelGraficos.SetActive(false);
        panelControles.SetActive(false);
        panelIdioma.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void VolverMenu()
    {
        panelOpciones.SetActive(false);
        panelCreditos.SetActive(false);
        panelPartidas.SetActive(false);
        if (panelNuevaPartida  != null) panelNuevaPartida.SetActive(false);
        if (panelCargarPartida != null) panelCargarPartida.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    // ══════════════════════════════════════════
    // SALIR
    // ══════════════════════════════════════════

    public void Salir()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
