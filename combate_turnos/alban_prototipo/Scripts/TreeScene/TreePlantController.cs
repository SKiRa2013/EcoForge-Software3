using UnityEngine;

public class TreePlantController : MonoBehaviour {
    public float timer;
    public float plantCooldown = 30f;
    public GameObject treePrefab;

    public int plantedNormal;
    public int plantedSeed;

    Vector3 GetMousePos() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        return worldPos;
    }

    // Start is called before the first frame update
    void Start() {
        timer = 0f;
        plantedNormal = 0;
        plantedSeed = 0;
    }

    // Update is called once per frame
    void Update() {
        if (timer > 1000f) timer = plantCooldown + 0.1f;
        
        timer += Time.deltaTime;
        
        if (timer >= plantCooldown) {
            if (Input.GetKeyDown("space")) {
                GameObject tree = Instantiate(treePrefab, GetMousePos(), Quaternion.identity);
                tree.GetComponentInChildren<TreeController>().isSeedPlanted = false;
                timer = 0f;
                plantedNormal++;
            } else if (Input.GetKeyDown(KeyCode.F)) {
                GameObject newTree = Instantiate(treePrefab, GetMousePos(), Quaternion.identity);
                newTree.GetComponentInChildren<TreeController>().isSeedPlanted = true;
                timer = 0f;
                plantedSeed++;
            }
        }
    }
}
