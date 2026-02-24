using UnityEngine;

public class EnemyForBattle : MonoBehaviour {
    public int quantity;
    public bool playerTurn;
    
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
    
    }    
}
