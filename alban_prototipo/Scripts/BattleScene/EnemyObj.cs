using UnityEngine;

public class EnemyObj : MonoBehaviour {
    public int atk, def, atkBonus, defBonus, hp;

    void Start() {
        hp = 1000;
        atk = 90;
        def = 20;

        atkBonus = 0;
        defBonus = 0;
    }
}