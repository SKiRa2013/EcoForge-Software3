using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlantController : MonoBehaviour {
    float timer;
    public float plantCooldown = 30f;
    public GameObject treePrefab;
    
    Vector3 GetMousePos() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        return mousePos;
    }

    // Start is called before the first frame update
    void Start() {
        timer = 0f;
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        if (timer >= plantCooldown) {
            if (Input.GetKeyDown("space")) {
                Instantiate(treePrefab, GetMousePos(), Quaternion.identity);
                timer = 0f;
            } else if (Input.GetKeyDown(KeyCode.F)) {
                GameObject newTree = Instantiate(treePrefab, GetMousePos(), Quaternion.identity);
                newTree.GetComponentInChildren<TreeController>().isSeedPlanted = true;
                timer = 0f;
            }
        }
    }
}
