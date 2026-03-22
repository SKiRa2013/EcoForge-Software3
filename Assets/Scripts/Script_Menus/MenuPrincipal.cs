using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMenuPrincipal;
    public GameObject panelPartidas;
    public GameObject panelOpciones;
    public GameObject panelAudio;
    public GameObject panelGraficos;
    public GameObject panelControles;
    public GameObject panelIdioma;
    public GameObject panelCreditos;
    public GameObject panelNuevaPartida;

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
        panelNuevaPartida.SetActive(false);
    }

    // ── Jugar → abre panel nueva partida ──
    public void IniciarPartida()
    {
        panelMenuPrincipal.SetActive(false);
        panelNuevaPartida.SetActive(true);

        // Llamar al NuevaPartidaPanel para inicializarlo
        var nuevaPartidaPanel = panelNuevaPartida.GetComponentInChildren<NuevaPartidaPanel>();
        if (nuevaPartidaPanel != null)
            nuevaPartidaPanel.AbrirNuevaPartida();
    }

    // ── Abrir panel de partidas (guardar y cargar) ──
    public void AbrirPartidas()
    {
        panelMenuPrincipal.SetActive(false);
        panelPartidas.SetActive(true);
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
        panelNuevaPartida.SetActive(false);
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