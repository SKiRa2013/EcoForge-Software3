using UnityEngine;
using UnityEngine.InputSystem;

public class PlantGrowth : MonoBehaviour
{
    [Header("Tipo de semilla")]
    [SerializeField] private TypeSeed tipoSemilla;

    [Header("Sprites")]
    [SerializeField] private Sprite flor;
    [SerializeField] private Sprite frutoPequeno;
    [SerializeField] private Sprite frutoGrande;

    private SpriteRenderer spriteRenderer;

    private float tiempoCrecimiento;
    private float tiempoFrutoPequeno;
    private float tiempoFrutoGrande;
    private float tiempoRegeneracion = 60f;

    private float timer;
    private int   etapa = 0;

    private bool listoParaCosechar  = false;
    private bool regenerando        = false;
    private bool jugadorCerca       = false;
    private bool crecimientoCompleto = false;

    private Vector3 escalaInicial = new Vector3(0.5f, 0.5f, 1f);
    private Vector3 escalaFinal   = new Vector3(1.5f, 1.5f, 1f);

    PlayerInput playerInput;
    InputAction cosecharAction;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = escalaInicial;

        playerInput   = FindFirstObjectByType<PlayerInput>();
        cosecharAction = playerInput.actions["Cosechar"];

        ConfigurarTipoSemilla();
    }

    void Update()
    {
        if (!crecimientoCompleto)
            CrecimientoInicial();
        else
            CicloFruto();

        if (jugadorCerca && listoParaCosechar && cosecharAction.WasPressedThisFrame())
            Cosechar();
    }

    void ConfigurarTipoSemilla()
    {
        switch (tipoSemilla)
        {
            case TypeSeed.Comun:   tiempoCrecimiento = 30f;  break;
            case TypeSeed.Red:     tiempoCrecimiento = 40f;  break;
            case TypeSeed.Green:   tiempoCrecimiento = 50f;  break;
            case TypeSeed.Blue:    tiempoCrecimiento = 60f;  break;
            case TypeSeed.Golden:  tiempoCrecimiento = 80f;  break;
        }

        tiempoFrutoPequeno = tiempoCrecimiento * 0.6f;
        tiempoFrutoGrande  = tiempoCrecimiento;
    }

    void CrecimientoInicial()
    {
        timer += Time.deltaTime;

        float progreso = timer / tiempoCrecimiento;
        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);

        if (timer >= tiempoFrutoPequeno && etapa == 0)
        {
            spriteRenderer.sprite = frutoPequeno;
            etapa = 1;
        }

        if (timer >= tiempoFrutoGrande && etapa == 1)
        {
            spriteRenderer.sprite = frutoGrande;
            etapa  = 2;
            listoParaCosechar  = true;
            crecimientoCompleto = true;
            Debug.Log("Fruto listo para cosechar");
        }
    }

    void CicloFruto()
    {
        if (regenerando)
        {
            timer += Time.deltaTime;

            if (timer >= tiempoRegeneracion)
            {
                regenerando = false;
                etapa  = 0;
                timer  = 0;
                spriteRenderer.sprite = flor;
                Debug.Log("La planta volvió a florecer");
            }
            return;
        }

        timer += Time.deltaTime;

        if (timer >= tiempoFrutoPequeno && etapa == 0)
        {
            spriteRenderer.sprite = frutoPequeno;
            etapa = 1;
        }

        if (timer >= tiempoFrutoGrande && etapa == 1)
        {
            spriteRenderer.sprite = frutoGrande;
            etapa = 2;
            listoParaCosechar = true;
            Debug.Log("Nuevo fruto listo para cosechar");
        }
    }

    void Cosechar()
    {
        int mana = ObtenerMana();
        Debug.Log("Fruto ingerido. Mana +" + mana);

        listoParaCosechar = false;
        regenerando       = true;
        timer             = 0;
    }

    int ObtenerMana()
    {
        switch (tipoSemilla)
        {
            case TypeSeed.Comun:   return 10;
            case TypeSeed.Red:     return 15;
            case TypeSeed.Green:   return 20;
            case TypeSeed.Blue:    return 30;
            case TypeSeed.Golden:  return 50;
        }
        return 0;
    }

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
}
