using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {
    public bool playerTurn, playerWin, enemyWin;
    public int quantity, step, enemyIndex;

    public List<GameObject> enemyTeam;
    public GameObject enemyPrefab, player;

    private EnemyForBattle battleSettings;
    private EnemyObj enemyClass;
    private PlayerObj playerClass;

    private GameObject enemyTarget;

    private string tipoHechizo;
    private string efecto;

    private int effectType, damage = -1;

    // Start is called before the first frame update
    void Start() {
        try {
            battleSettings = GameObject.Find("BattleExport").GetComponent<EnemyForBattle>();
            quantity = battleSettings.quantity;
            playerTurn = battleSettings.playerTurn;
        }

        catch {
            quantity = 3;
            playerTurn = true;
        }
        
        playerWin = false;
        enemyWin = false;

        playerClass = player.GetComponent<PlayerObj>();

        for (int i = 0; i < quantity; i++) {
            enemyTeam.Add(Instantiate(enemyPrefab, new Vector3(1.5f + 2 * i, -0.5f, 1), Quaternion.identity));
        }

        step = 0;
        enemyIndex = -1;
        enemyTarget = null;

        tipoHechizo = "Jugador lanza: ";
        efecto = "";
        effectType = -1;
    }

    void EnemyTurn() {
        foreach (GameObject enemy in enemyTeam) {
            Debug.LogWarning("Enemigo ataca");
            enemyClass = enemy.GetComponent<EnemyObj>();

            int damage = (enemyClass.atk + enemyClass.atkBonus) - (playerClass.def + playerClass.defBonus);
            if (damage < 0) damage = 0;

            playerClass.hp -= damage;
            if (playerClass.hp < 0) playerClass.hp = 0;

            Debug.LogWarning($"Produce {damage} de daño al jugador, Vida Restante: {playerClass.hp} HP");

            if (playerClass.hp <= 0) {
                enemyWin = true;
                Debug.LogWarning($"El jugador ha muerto, Vida: {playerClass.hp}");
                Destroy(player);
                break;
            }
        }

        if (enemyWin) return;    
        playerTurn = !playerTurn;
    }

    // Update is called once per frame
    void Update() {
        if (playerWin || enemyWin) return;
        if (!playerTurn) EnemyTurn();

        if (step == 0) {
            Debug.Log("Turno del jugador, número para elegir hechizo (1-6): ");

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                tipoHechizo += "dañoLeve";
                effectType = 1;
                step = 1;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                tipoHechizo += "dañoFuerte";
                effectType = 2;
                step = 1;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                tipoHechizo += "buffLeve";
                effectType = 0;

                playerClass.atkBonus += 10;
                playerClass.defBonus += 10;

                efecto = $" Jugador aumenta sus estadísticas. Atk: Leve a {10 + playerClass.atkBonus}, Fuerte a {30 + playerClass.atkBonus}, Def: {playerClass.defBonus + playerClass.def}";
                step = 2;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                tipoHechizo += "buffFuerte";
                effectType = 0;

                playerClass.atkBonus += 30;
                playerClass.defBonus += 30;

                efecto = $" Jugador aumenta sus estadísticas. Atk: Leve a {10 + playerClass.atkBonus}, Fuerte a {30 + playerClass.atkBonus}, Def: {playerClass.defBonus + playerClass.def}";
                step = 2;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha5)) {
                tipoHechizo += "debuffLeve";
                effectType = 3;
                step = 1;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha6)) {
                tipoHechizo += "debuffFuerte";
                effectType = 4;
                step = 1;
            }
        }

        else if (step == 1) {
            if (enemyTeam.Count > 1) Debug.LogError($"Turno del jugador, número para elegir objetivo (1-{enemyTeam.Count}): ");
            else Debug.LogError($"Atacando al único enemigo restante");

            if (Input.GetKeyDown(KeyCode.Alpha1) || enemyTeam.Count < 2) {
                enemyIndex = 0;
                step = 2;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2) && enemyTeam.Count >= 2) {
                enemyIndex = 1;
                step = 2;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha3) && enemyTeam.Count >= 3) {
                enemyIndex = 2;
                step = 2;
            }

            else if (Input.GetKeyDown(KeyCode.Alpha4) && enemyTeam.Count >= 4) {
                enemyIndex = 3;
                step = 2;
            }

            if (step == 2) {
                enemyTarget = enemyTeam[enemyIndex];
                enemyClass = enemyTarget.GetComponent<EnemyObj>();
            
                switch(effectType) {
                    // Daño leve
                    case 1:
                        damage = 30 + playerClass.atkBonus - (enemyClass.def + enemyClass.defBonus);
                        if (damage < 0) damage = 0;

                        enemyClass.hp -= damage;
                        if (enemyClass.hp < 0) enemyClass.hp = 0;

                        efecto = $" Enemigo {enemyIndex+1} recibe {damage} de daño. Vida restante: {enemyClass.hp}";
                        break;

                    // Daño fuerte
                    case 2:
                        damage = 70 + playerClass.atkBonus - (enemyClass.def + enemyClass.defBonus);
                        if (damage < 0) damage = 0;

                        enemyClass.hp -= damage;
                        if (enemyClass.hp < 0) enemyClass.hp = 0;

                        efecto = $" Enemigo {enemyIndex+1} recibe {damage} de daño. Vida restante: {enemyClass.hp}";
                        break;

                    // Debuff leve
                    case 3:
                        enemyClass.atkBonus -= 12;
                        enemyClass.defBonus -= 12;

                        if (enemyClass.atk + enemyClass.atkBonus < 0) enemyClass.atkBonus = -enemyClass.atk;
                        if (enemyClass.def + enemyClass.defBonus < 0) enemyClass.defBonus = -enemyClass.def;

                        efecto = $" Estadísticas de enemigo {enemyIndex+1} reducidas en -12. Atk: {enemyClass.atkBonus - enemyClass.atk}, Def: {enemyClass.defBonus - enemyClass.def}";
                        break;

                    // Debuff fuerte
                    case 4:
                        enemyClass.atkBonus -= 15;
                        enemyClass.defBonus -= 15;

                        if (enemyClass.atk + enemyClass.atkBonus < 0) enemyClass.atkBonus = -enemyClass.atk;
                        if (enemyClass.def + enemyClass.defBonus < 0) enemyClass.defBonus = -enemyClass.def;

                        efecto = $" Estadísticas de enemigo {enemyIndex+1} reducidas en -15. Atk: {enemyClass.atkBonus - enemyClass.atk}, Def: {enemyClass.defBonus - enemyClass.def}";
                        break;
                }
            }
        }
        
        else if (step == 2) {
            Debug.LogWarning($"{tipoHechizo}. {efecto}");

            if (effectType > 0) {
                if (enemyClass.hp <= 0) {
                    Destroy(enemyTeam[enemyIndex]);
                    enemyTeam.RemoveAt(enemyIndex);
                }
            }

            if (enemyTeam.Count <= 0) {
                playerWin = true;
                Debug.LogWarning("GANASTE. Aquí harías un SceneManager.LoadScene(Scene) con la escena de los mapas, asegúrate de guardar el valor de maná en un objeto y usar DontDestroyOnLoad(GameObject)");
                return;
            }

            playerTurn = !playerTurn;

            step = 0;
            enemyIndex = -1;
            enemyTarget = null;

            tipoHechizo = "Jugador lanza: ";
            efecto = "";
            effectType = -1;
        }  
    }
}
