using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SistemaCombate : MonoBehaviour
{
    [Header("Referencias a UI")]
    public Text healthBarText;
    public Text manaBarText;
    public Text healthBarEnemyText;
    public GameObject panelBotones; 
    [Header("Botones de Ataque")]
    public Button[] botones; 
    [Header("Stats de la Bruja (Protagonista)")]
    public float brujaVida = 1000f;
    public float brujaMana = 1000f;
    public float brujaFuerza = 100f; 
    public float brujaDefensa = 20f;
    
    [Header("Stats del Caballero (Enemigo)")]
    public float caballeroVida = 1000f;
    public float caballeroFuerza = 100f;
    public float caballeroDefensa = 20f;
    
    [Header("Control de Turnos")]
    public bool turnoJugador = true;
    public bool combateActivo = true;

    void Start()
    {

        VerificarReferencias();
        

        AsignarListenersBotones();
        

        ActualizarTodasLasBarras();
        

        IniciarTurnoJugador();
    }

    void VerificarReferencias()
    {
        if (healthBarText == null) Debug.LogError("Health Bar Text no asignado");
        if (manaBarText == null) Debug.LogError("Mana Bar Text no asignado");
        if (healthBarEnemyText == null) Debug.LogError("Health Bar Enemy Text no asignado");
        if (botones.Length != 6) Debug.LogError("Se necesitan exactamente 6 botones");
    }

    void AsignarListenersBotones()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] != null)
            {
                int indice = i;
                botones[i].onClick.AddListener(() => EjecutarAccionJugador(indice));
            }
        }
    }

    void IniciarTurnoJugador()
    {
        turnoJugador = true;
        Debug.Log(" Turno del jugador - Selecciona una acción");
    }

    void EjecutarAccionJugador(int indiceBoton)
    {
        if (!turnoJugador || !combateActivo)
        {
            Debug.Log("No es tu turno o el combate ha terminado");
            return;
        }


        switch (indiceBoton)
        {
            case 0: 
                AtaqueDebil();
                break;
            case 1: 
                AtaqueFuerte();
                break;
            case 2:
                BuffDebil();
                break;
            case 3: 
                BuffFuerte();
                break;
            case 4: 
                DebuffDebil();
                break;
            case 5: 
                DebuffFuerte();
                break;
        }
        

        turnoJugador = false;
        StartCoroutine(TurnoEnemigo());
    }



    void AtaqueDebil()
    {
        int dañoBase = Random.Range(100, 201);
        float dañoFinal = CalcularDaño(dañoBase, brujaFuerza, caballeroDefensa);
        
        caballeroVida = Mathf.Max(0, caballeroVida - dañoFinal);
        ActualizarBarraEnemigo();
        
        Debug.Log($" Ataque Débil: {dañoBase} base -> {dañoFinal:F1} daño final");
    }

    void AtaqueFuerte()
    {
        int dañoBase = Random.Range(250, 751);
        float dañoFinal = CalcularDaño(dañoBase, brujaFuerza, caballeroDefensa);
        
        caballeroVida = Mathf.Max(0, caballeroVida - dañoFinal);
        ActualizarBarraEnemigo();
        
        Debug.Log($" Ataque Fuerte: {dañoBase} base -> {dañoFinal:F1} daño final");
    }



    void BuffDebil()
    {

        brujaFuerza += 10f;
        brujaDefensa += 10f;
        
        ActualizarStatsJugador();
        Debug.Log($" Buff Débil: ATK +10, DEF +10 (ATK: {brujaFuerza}, DEF: {brujaDefensa})");
    }

    void BuffFuerte()
    {
        int aumentoATK = Random.Range(25, 51);
        int aumentoDEF = Random.Range(25, 51); 
        
        brujaFuerza += aumentoATK;
        brujaDefensa += aumentoDEF;
        
        ActualizarStatsJugador();
        Debug.Log($" Buff Fuerte: ATK +{aumentoATK}, DEF +{aumentoDEF} (ATK: {brujaFuerza}, DEF: {brujaDefensa})");
    }



    void DebuffDebil()
    {

        caballeroFuerza = Mathf.Max(0, caballeroFuerza - 5f);
        caballeroDefensa = Mathf.Max(0, caballeroDefensa - 5f);
        
        ActualizarStatsEnemigo();
        Debug.Log($" Debuff Débil enemigo: ATK -5, DEF -5 (ATK: {caballeroFuerza}, DEF: {caballeroDefensa})");
    }

    void DebuffFuerte()
    {
        int reduccionATK = Random.Range(5, 11);
        int reduccionDEF = Random.Range(5, 11);
        
        caballeroFuerza = Mathf.Max(0, caballeroFuerza - reduccionATK);
        caballeroDefensa = Mathf.Max(0, caballeroDefensa - reduccionDEF);
        
        ActualizarStatsEnemigo();
        Debug.Log($" Debuff Fuerte enemigo: ATK -{reduccionATK}, DEF -{reduccionDEF} (ATK: {caballeroFuerza}, DEF: {caballeroDefensa})");
    }



    IEnumerator TurnoEnemigo()
    {
        yield return new WaitForSeconds(1f); 
        
        if (!combateActivo) yield break;
        
        Debug.Log(" Turno del Caballero");
        

        float manaConsumido = 50f;
        brujaMana = Mathf.Max(0, brujaMana - manaConsumido);
        ActualizarMana();
        
        // Enemigo ataca
        int dañoEnemigo = Random.Range(20, 51); 
        float dañoFinal = CalcularDaño(dañoEnemigo, caballeroFuerza, brujaDefensa);
        
        brujaVida = Mathf.Max(0, brujaVida - dañoFinal);
        ActualizarVidaJugador();
        
        Debug.Log($" Ataque enemigo: {dañoEnemigo} base -> {dañoFinal:F1} daño a la bruja");
        
 
        if (VerificarFinCombate())
        {
            yield break;
        }
        

        IniciarTurnoJugador();
    }



    float CalcularDaño(float dañoBase, float fuerzaAtacante, float defensaObjetivo)
    {

        float multiplicadorFuerza = fuerzaAtacante / 100f;

        float reduccionDefensa = 1f - (defensaObjetivo * 0.001f);
        
        float dañoCalculado = dañoBase * multiplicadorFuerza * reduccionDefensa;
        return Mathf.Max(1, dañoCalculado); 
    }

    bool VerificarFinCombate()
    {
        if (brujaVida <= 0)
        {
            Debug.Log(" ¡Has sido derrotado!");
            combateActivo = false;
            return true;
        }
        
        if (caballeroVida <= 0)
        {
            Debug.Log(" ¡Has derrotado al caballero!");
            combateActivo = false;
            return true;
        }
        
        return false;
    }



    void ActualizarTodasLasBarras()
    {
        ActualizarVidaJugador();
        ActualizarMana();
        ActualizarBarraEnemigo();
    }

    void ActualizarVidaJugador()
    {
        if (healthBarText != null)
        {
            healthBarText.text = $"VIDA: {brujaVida:F0}/1000";
        }
    }

    void ActualizarMana()
    {
        if (manaBarText != null)
        {
            manaBarText.text = $"MANA: {brujaMana:F0}/1000";
        }
    }

    void ActualizarBarraEnemigo()
    {
        if (healthBarEnemyText != null)
        {
            healthBarEnemyText.text = $"VIDA ENEMIGO: {caballeroVida:F0}/1000";
        }
    }

    void ActualizarStatsJugador()
    {
        Debug.Log($" Stats Bruja - Vida: {brujaVida:F0}, Mana: {brujaMana:F0}, Fuerza: {brujaFuerza:F1}, Defensa: {brujaDefensa:F1}");
    }

    void ActualizarStatsEnemigo()
    {
        Debug.Log($" Stats Caballero - Vida: {caballeroVida:F0}, Fuerza: {caballeroFuerza:F1}, Defensa: {caballeroDefensa:F1}");
    }
}