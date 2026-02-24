using UnityEngine;

public class TreeController : MonoBehaviour {
    public bool isSeedPlanted;
    public bool isValid;
    
    float timer;

    public GameObject objBackground;

    // Start is called before the first frame update
    void Start() {
        isValid = false;
        objBackground = GameObject.Find("background");
    }

    // Update is called once per frame
    void Update() {
        if (isSeedPlanted) return;

        if(timer < 0.2f) timer += Time.deltaTime;

        if (timer >= 0.2f) {
            if (!isValid) {
                objBackground.GetComponent<TreePlantController>().plantedNormal--;
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        if (isValid) return;

        isValid = other.CompareTag("TreeArea") && other.transform.parent != transform.parent;
    }
}