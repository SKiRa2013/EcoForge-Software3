using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour {
    private GameObject player;
    public int enemyQuantity;
    private Animator anim;

    public float speed;
    public bool arrived;

    public float stopDistance;

    private Vector3 previousPos;
    private Vector2 currentVelocity, direction;

    public char orientation;

    public GameObject enemyExport;

    void Start() {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        arrived = false;

        speed = 120f;
        stopDistance = 40f;
        orientation = 'F';

        enemyExport = GameObject.Find("BattleExport");
    }

    void Update() {
        if (player == null) return;
        if (arrived) return;

        previousPos = transform.position;
        direction = (player.transform.position - transform.position).normalized;

        // Raycast para evitar obstáculos (árboles)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f);

        if (hit.collider != null && hit.collider.CompareTag("Tree")) {
            // Si hay un árbol, intentamos rodearlo desviando la dirección
            direction += (new Vector2(direction.y, -direction.x) * 40f).normalized;
        }

        if (Vector3.Distance(player.transform.position, transform.position) < stopDistance) {
            arrived = true;
            currentVelocity = Vector2.zero;
            transform.Translate(0, 0, 0);

            StartBattle();
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime);
        currentVelocity = ((Vector2)transform.position - (Vector2)previousPos) / Time.deltaTime;
        arrived = false;

        ActualizarAnimaciones();
    }

    void StartBattle() {
        char playerOrientation = player.GetComponent<PlayerController>().orientation;

        Debug.LogWarning($"Orientación => Jugador: {playerOrientation}, Enemigos: {orientation}");

        // Vertical, turno para jugador
        if ((playerOrientation == 'F' && orientation == 'B') || (playerOrientation == 'B' && orientation == 'F')) {
            Debug.LogWarning("Turno para jugador - Vertical");
            enemyExport.GetComponent<EnemyForBattle>().playerTurn = true;
            SceneManager.LoadScene("BattleScene");
            return;
        }

        // Horizontal, turno para jugador
        if ((playerOrientation == 'L' && orientation == 'R') || (playerOrientation == 'R' && orientation == 'L')) {
            Debug.LogWarning("Turno para jugador - Horizontal");
            enemyExport.GetComponent<EnemyForBattle>().playerTurn = true;
            SceneManager.LoadScene("BattleScene");
            return;
        }

        // Turno para enemigos
        Debug.LogWarning("Turno para enemigos");
        enemyExport.GetComponent<EnemyForBattle>().playerTurn = false;
        SceneManager.LoadScene("BattleScene");
    }

    void ActualizarAnimaciones() {
        // Obtenemos la dirección del movimiento basándonos en la velocidad del agente
        Vector2 velocity = currentVelocity;
        int horizontal = (short)(velocity.x / Math.Abs(velocity.x));
        int vertical = (short)(velocity.y / Math.Abs(velocity.y));

        if (velocity.magnitude > 0.1f) {
            anim.SetBool("isMoving", true);

            if (Math.Abs(velocity.x) > Math.Abs(velocity.y)) {
                orientation = new char[] { 'L', 'R' }[(horizontal + 1) / 2];
                
                anim.SetInteger("horizontal", horizontal);
                anim.SetInteger("vertical", 0);
                return;
            }

            orientation = new char[] { 'B', 'F' }[(vertical + 1) / 2];

            anim.SetInteger("vertical", vertical);
            anim.SetInteger("horizontal", 0);
            return;
        }

        else {
            Debug.Log("ANIM: Idle");
            anim.SetBool("isMoving", false);
            anim.SetInteger("horizontal", 0);
            anim.SetInteger("vertical", 0);
        }
    }
}
