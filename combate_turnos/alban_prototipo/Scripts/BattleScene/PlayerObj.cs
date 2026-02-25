using UnityEngine;

public class PlayerObj : MonoBehaviour {
    public int atkBonus, def, defBonus, mana, hp;

    void Start() {
        hp = 1000;
        mana = 1000;
        def = 20;

        atkBonus = 0;
        defBonus = 0;
    }    
}