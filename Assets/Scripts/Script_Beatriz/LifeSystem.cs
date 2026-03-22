using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BattleSystem : MonoBehaviour
{
    [Header("PLAYER BASE")]
    public int playerMaxHP = 1000;
    public int playerHP = 1000;
    public int playerBaseDEF = 20;

    int playerBuffATK = 0;
    int playerBuffDEF = 0;

    [Header("ENEMY BASE")]
    public int enemyMaxHP = 1000;
    public int enemyHP = 1000;

    int enemyBaseATK;
    int enemyBaseDEF;

    int enemyDebuffATK = 0;
    int enemyDebuffDEF = 0;

    [Header("UI")]
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;

    [Header("LIFE SPRITES (vida_0 a vida_9)")]
    public Image lifeImage;
    public Sprite[] lifeSprites;

    void Start()
    {
        playerHP = playerMaxHP;
        enemyHP = enemyMaxHP;

        enemyBaseATK = Random.Range(20, 51);
        enemyBaseDEF = Random.Range(20, 31);

        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // =====================
        // ATAQUES
        // =====================
        if (Keyboard.current.qKey.wasPressedThisFrame)
            BasicAttack();

        if (Keyboard.current.wKey.wasPressedThisFrame)
            StrongAttack();

        // =====================
        // BUFFS
        // =====================
        if (Keyboard.current.eKey.wasPressedThisFrame)
            BasicBuff();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            StrongBuff();

        // =====================
        // DEBUFFS
        // =====================
        if (Keyboard.current.tKey.wasPressedThisFrame)
            BasicDebuff();

        if (Keyboard.current.yKey.wasPressedThisFrame)
            StrongDebuff();

        // =====================
        // 🔥 TEST VISUAL PLAYER
        // =====================
        if (Keyboard.current.uKey.wasPressedThisFrame)
            SimulatePlayerDamage();

        if (Keyboard.current.iKey.wasPressedThisFrame)
            SimulatePlayerHeal();
    }

    // ===============================
    // SIMULACIÓN VISUAL
    // ===============================

    void SimulatePlayerDamage()
    {
        int damage = Random.Range(100, 301);
        playerHP -= damage;
        playerHP = Mathf.Clamp(playerHP, 0, playerMaxHP);
        UpdateUI();
    }

    void SimulatePlayerHeal()
    {
        int heal = Random.Range(100, 301);
        playerHP += heal;
        playerHP = Mathf.Clamp(playerHP, 0, playerMaxHP);
        UpdateUI();
    }

    // ===============================
    // ATAQUES
    // ===============================

    void BasicAttack()
    {
        int damage = Random.Range(100, 201);
        damage += playerBuffATK;
        ApplyDamageToEnemy(damage);
    }

    void StrongAttack()
    {
        int damage = Random.Range(250, 751);
        damage += playerBuffATK;
        ApplyDamageToEnemy(damage);
    }

    void ApplyDamageToEnemy(int damage)
    {
        int totalDEF = enemyBaseDEF - enemyDebuffDEF;
        int finalDamage = Mathf.Max(damage - totalDEF, 0);

        enemyHP -= finalDamage;
        enemyHP = Mathf.Clamp(enemyHP, 0, enemyMaxHP);

        UpdateUI();
    }

    // ===============================
    // BUFFS
    // ===============================

    void BasicBuff()
    {
        playerBuffATK += 10;
        playerBuffDEF += 10;
    }

    void StrongBuff()
    {
        int atkBuff = Random.Range(25, 51);
        int defBuff = Random.Range(25, 51);

        playerBuffATK += atkBuff;
        playerBuffDEF += defBuff;
    }

    // ===============================
    // DEBUFFS
    // ===============================

    void BasicDebuff()
    {
        enemyDebuffATK += 5;
        enemyDebuffDEF += 5;
    }

    void StrongDebuff()
    {
        int debuff = Random.Range(5, 11);

        enemyDebuffATK += debuff;
        enemyDebuffDEF += debuff;
    }

    // ===============================
    // UI
    // ===============================

    void UpdateUI()
    {
        if (playerHPText != null)
            playerHPText.text = playerHP.ToString();

        if (enemyHPText != null)
            enemyHPText.text = enemyHP.ToString();

        UpdateLifeSprite();
    }

    void UpdateLifeSprite()
    {
        if (lifeImage == null || lifeSprites.Length == 0)
            return;

        float percentage = (float)playerHP / playerMaxHP;
        int spriteIndex = Mathf.RoundToInt(percentage * 9);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, 9);

        lifeImage.sprite = lifeSprites[spriteIndex];
    }
}