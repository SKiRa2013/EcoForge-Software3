using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    public int spawnProb;
    public int spawnQuantity;
    public int treesPlanted;

    public int baseProb;
    public int plantRisk;
    public float timer;
    public float spawnCalcCooldown;

    public GameObject enemyPrefab;

    public float spawnHeight, spawnWidth, spawnAreaMargin;
    public int generatedNumber;

    public bool enemySpawned;

    private TreePlantController plantController;
    private Camera cam;

    public GameObject battleExport;

    // Start is called before the first frame update
    void Start() {
        plantController = gameObject.GetComponent<TreePlantController>();
        baseProb = 10;
        plantRisk = 2;
        timer = 0f;
        spawnCalcCooldown = 10f;

        enemySpawned = false;

        cam = Camera.main;
    }

    Vector2 GetWorldBounds() {
        // Distancia desde la cámara al plano 0 (donde ocurre el juego)
        float distance = Mathf.Abs(cam.transform.position.z);

        // Esquina inferior izquierda (0,0) y superior derecha (1,1) en espacio de pantalla
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));

        // Retornamos el tamaño (Ancho, Alto)
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        
        return new Vector2(width, height);
    }

    // Update is called once per frame
    void Update() {
        if (enemySpawned) return;

        timer += Time.deltaTime;

        if (timer >= spawnCalcCooldown) {
            timer = 0f;
            
            treesPlanted = plantController.plantedNormal + plantController.plantedSeed;
            spawnProb = baseProb + (plantRisk * treesPlanted);

            if (spawnProb > 100) spawnProb = 100;

            generatedNumber = Random.Range(1, 101);

            if (generatedNumber <= spawnProb) {
                Vector2 bounds = GetWorldBounds();
                spawnWidth = (bounds.x / 2) + spawnAreaMargin;
                spawnHeight = (bounds.y / 2) + spawnAreaMargin;

                Vector3 enemyPos = new Vector3(
                    Random.Range(-spawnWidth, spawnWidth),
                    Random.Range(-spawnHeight, spawnHeight),
                    0
                );

                GameObject enemy;

                if (Mathf.Abs(enemyPos.x) > (bounds.x / 2) || Mathf.Abs(enemyPos.y) > (bounds.y / 2)) {
                    enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
                }

                else {
                    enemyPos.x += (enemyPos.x > 0) ? spawnAreaMargin : -spawnAreaMargin;
                    enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
                }

                spawnQuantity = (int)(Random.Range(0, 6) / 2.5f) + 2;
                enemy.GetComponent<EnemyController>().enemyQuantity = spawnQuantity;

                enemySpawned = true;

                Debug.LogWarning($"Enemigos: {spawnQuantity}");

                battleExport.GetComponent<EnemyForBattle>().quantity = spawnQuantity;
            } 
            
        }
    }
}
