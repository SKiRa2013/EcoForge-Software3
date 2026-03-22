using UnityEngine;
using UnityEngine.UI;

public class OpcionesManager : MonoBehaviour
{
    [Header("Subpaneles de contenido")]
    public GameObject subPanelGeneral;
    public GameObject subPanelGraficos;
    public GameObject subPanelControles;
    public GameObject subPanelAudio;

    [Header("Botones de navegación")]
    public Button btnGeneral;
    public Button btnGraficos;
    public Button btnControles;
    public Button btnAudio;

    [Header("Colores de botón")]
    public Color colorActivo   = new Color(0.10f, 0.29f, 0.10f, 1f); // #1a4a1a
    public Color colorInactivo = new Color(0.06f, 0.12f, 0.06f, 0.78f); // #0F1E0F

    GameObject subPanelActual;
    Button     btnActual;

    void Start()
    {
        // Iniciar con General activo
        MostrarGeneral();
    }

    // ══════════════════════════════════════════
    // BOTONES DE NAVEGACIÓN
    // ══════════════════════════════════════════

    public void MostrarGeneral()
    {
        CambiarPanel(subPanelGeneral, btnGeneral);
    }

    public void MostrarGraficos()
    {
        CambiarPanel(subPanelGraficos, btnGraficos);
    }

    public void MostrarControles()
    {
        CambiarPanel(subPanelControles, btnControles);
    }

    public void MostrarAudio()
    {
        CambiarPanel(subPanelAudio, btnAudio);
    }

    // ══════════════════════════════════════════
    // LÓGICA CENTRAL
    // ══════════════════════════════════════════

    void CambiarPanel(GameObject nuevoPanel, Button nuevoBtn)
    {
        // Ocultar panel actual
        if (subPanelActual != null)
            subPanelActual.SetActive(false);

        // Desactivar color del botón actual
        if (btnActual != null)
            btnActual.GetComponent<Image>().color = colorInactivo;

        // Activar nuevo panel
        nuevoPanel.SetActive(true);

        // Activar color del nuevo botón
        if (nuevoBtn != null)
            nuevoBtn.GetComponent<Image>().color = colorActivo;

        // Guardar referencia
        subPanelActual = nuevoPanel;
        btnActual      = nuevoBtn;
    }
}
