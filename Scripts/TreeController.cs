using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;

public class TreeController : MonoBehaviour {
    public bool isSeedPlanted;
    bool isValid;
    float timer;

    // Start is called before the first frame update
    void Start() {
        isValid = false;
    }

    // Update is called once per frame
    void Update() {
        if (isSeedPlanted) return;
        
        timer += Time.deltaTime;

        if (timer >= 0.2f) {
            timer = 0f;

            if (!isValid) {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isValid) return;

        isValid = other.CompareTag("TreeArea") && other.transform.parent != transform.parent;
    }
}
