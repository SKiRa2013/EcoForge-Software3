using UnityEngine;
using UnityEngine.InputSystem;

public class PlantGrowth : MonoBehaviour
{
    // ══════════════════════════════════════════
    // CONFIGURACIÓN
    // ══════════════════════════════════════════

    [Header("Tipo de semilla")]
    [SerializeField] private TypeSeed tipoSemilla;

    [Header("Sprites del árbol (5 etapas)")]
    [SerializeField] private Sprite[] spritesArbol; // 0=brote, 1, 2, 3, 4=adulto

    [Header("Sprites del fruto (5 etapas)")]
    [SerializeField] private Sprite[] spritesFruto; // 0=pequeño ... 4=maduro

    [Header("Tiempos de crecimiento")]
    [SerializeField] private float tiempoPorEtapaArbol = 15f;  // segundos por etapa del árbol
    [SerializeField] private float tiempoPorEtapaFruto = 10f;  // segundos por etapa del fruto
    [SerializeField] private float tiempoRegeneracion  = 30f;  // segundos para que vuelva el fruto

    // ── Componentes ──
    private SpriteRenderer spriteRendererArbol;
    private SpriteRenderer spriteRendererFruto;

    // ── Estado del árbol ──
    private int  etapaArbol      = 0;
    private bool arbolMaduro     = false;
    private float timerArbol     = 0f;

    // ── Estado del fruto ──
    private int   etapaFruto         = 0;
    private bool  frutoCreciendo     = false;
    private bool  frutoListo         = false;
    private bool  regenerandoFruto   = false;
    private float timerFruto         = 0f;

    // ── Interacción ──
    private bool jugadorCerca = false;

    // ── Input ──
    InputAction cosecharAction;

    // ══════════════════════════════════════════
    // SETUP
    // ══════════════════════════════════════════

    void Start()
    {
        // Buscar SpriteRenderers incluyendo objetos inactivos
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);

        if (renderers.Length >= 2)
        {
            spriteRendererArbol = renderers[0];
            spriteRendererFruto = renderers[1];
            spriteRendererFruto.sortingOrder = spriteRendererArbol.sortingOrder + 1;
        }
        else if (renderers.Length == 1)
        {
            spriteRendererArbol = renderers[0];
            Debug.LogWarning("Falta el hijo Fruto en el prefab: " + gameObject.name);
        }
        else
        {
            spriteRendererArbol = GetComponent<SpriteRenderer>();
        }

        // Ocultar fruto al inicio
        if (spriteRendererFruto != null)
            spriteRendererFruto.enabled = false;

        // Mostrar etapa 0 del árbol
        ActualizarSpriteArbol();

        // Input
        var playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
            cosecharAction = playerInput.actions["Cosechar"];
    }

    // ══════════════════════════════════════════
    // UPDATE
    // ══════════════════════════════════════════

    void Update()
    {
        if (!arbolMaduro)
            CrecerArbol();
        else
            ManejarFruto();

        // Cosechar
        if (jugadorCerca && frutoListo && cosecharAction != null
            && cosecharAction.triggered)
        {
            Cosechar();
        }
    }

    // ══════════════════════════════════════════
    // CRECIMIENTO DEL ÁRBOL
    // ══════════════════════════════════════════

    void CrecerArbol()
    {
        timerArbol += Time.deltaTime;

        if (timerArbol >= tiempoPorEtapaArbol)
        {
            timerArbol = 0f;
            etapaArbol++;

            if (etapaArbol >= 5)
            {
                etapaArbol  = 4;
                arbolMaduro = true;
                frutoCreciendo = true;
                timerFruto  = 0f;
                etapaFruto  = 0;

                Debug.Log("Árbol maduro — iniciando crecimiento de fruto");
            }

            ActualizarSpriteArbol();
        }
    }

    // ══════════════════════════════════════════
    // MANEJO DEL FRUTO
    // ══════════════════════════════════════════

    void ManejarFruto()
    {
        // ── Creciendo fruto ──
        if (frutoCreciendo)
        {
            timerFruto += Time.deltaTime;

            // Mostrar fruto desde etapa 0 (aparece con árbol en etapa 3 → índice 2)
            if (spriteRendererFruto != null && !spriteRendererFruto.enabled)
                spriteRendererFruto.enabled = true;

            if (timerFruto >= tiempoPorEtapaFruto)
            {
                timerFruto = 0f;
                etapaFruto++;

                if (etapaFruto >= 5)
                {
                    etapaFruto     = 4;
                    frutoCreciendo = false;
                    frutoListo     = true;
                    Debug.Log("Fruto listo para cosechar");
                }

                ActualizarSpriteFruto();
            }
        }

        // ── Regenerando fruto ──
        if (regenerandoFruto)
        {
            timerFruto += Time.deltaTime;

            if (timerFruto >= tiempoRegeneracion)
            {
                timerFruto     = 0f;
                regenerandoFruto = false;
                frutoCreciendo = true;
                etapaFruto     = 0;

                if (spriteRendererFruto != null)
                    spriteRendererFruto.enabled = true;

                ActualizarSpriteFruto();
                Debug.Log("Fruto regenerando");
            }
        }
    }

    // ══════════════════════════════════════════
    // COSECHAR
    // ══════════════════════════════════════════

    void Cosechar()
    {
        int mana = ObtenerMana();

        // Dar mana al jugador
        var manaSystem = FindFirstObjectByType<ManaSystem>();
        if (manaSystem != null)
            manaSystem.AddMana(mana);

        Debug.Log("Fruto cosechado. Mana +" + mana);

        // Ocultar fruto
        if (spriteRendererFruto != null)
            spriteRendererFruto.enabled = false;

        // Árbol queda en sprite 5 (índice 4) sin fruto
        etapaArbol = 4;
        ActualizarSpriteArbol();

        // Iniciar regeneración
        frutoListo       = false;
        frutoCreciendo   = false;
        regenerandoFruto = true;
        timerFruto       = 0f;
        etapaFruto       = 0;
    }

    // ══════════════════════════════════════════
    // ACTUALIZAR SPRITES
    // ══════════════════════════════════════════

    void ActualizarSpriteArbol()
    {
        if (spriteRendererArbol == null || spritesArbol == null) return;
        if (etapaArbol < 0 || etapaArbol >= spritesArbol.Length) return;

        spriteRendererArbol.sprite = spritesArbol[etapaArbol];
    }

    void ActualizarSpriteFruto()
    {
        if (spriteRendererFruto == null || spritesFruto == null) return;
        if (etapaFruto < 0 || etapaFruto >= spritesFruto.Length) return;

        spriteRendererFruto.sprite = spritesFruto[etapaFruto];
    }

    // ══════════════════════════════════════════
    // TRIGGER
    // ══════════════════════════════════════════

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorCerca = false;
    }

    // ══════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════

    int ObtenerMana()
    {
        switch (tipoSemilla)
        {
            case TypeSeed.Comun:  return 10;
            case TypeSeed.Red:    return 15;
            case TypeSeed.Green:  return 20;
            case TypeSeed.Blue:   return 30;
            case TypeSeed.Golden: return 50;
        }
        return 0;
    }
}