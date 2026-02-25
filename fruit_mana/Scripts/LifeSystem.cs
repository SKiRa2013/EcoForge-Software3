using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    // =========================
    // PLAYER
    // =========================

    [Header("PLAYER")]
    public int playerMaxHP = 1000;
    public int playerHP;

    public int playerBaseDEF = 20;
    public int playerBuffATK = 0;
    public int playerBuffDEF = 0;

    // =========================
    // ENEMY
    // =========================

    [Header("ENEMY")]
    public int enemyMaxHP = 1000;
    public int enemyHP;

    public int enemyATK;
    public int enemyDEF;

    // =========================
    // UI
    // =========================

    [Header("UI TEXT")]
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;

    [Header("LIFE SPRITES")]
    public Image lifeImage;
    public Sprite[] lifeSprites; // 0-9

    void Start()
    {
        playerHP = playerMaxHP;
        enemyHP = enemyMaxHP;

        // Valores iniciales del enemigo
        enemyATK = Random.Range(20, 51);
        enemyDEF = Random.Range(20, 31);

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BasicDamage();
        if (Input.GetKeyDown(KeyCode.Alpha2)) StrongDamage();
        if (Input.GetKeyDown(KeyCode.Alpha3)) BasicBuff();
        if (Input.GetKeyDown(KeyCode.Alpha4)) StrongBuff();
        if (Input.GetKeyDown(KeyCode.Alpha5)) BasicDebuff();
        if (Input.GetKeyDown(KeyCode.Alpha6)) StrongDebuff();
        if (Input.GetKeyDown(KeyCode.Alpha7)) EnemyAttack();
    }

    // =========================
    // HECHIZOS DE DAÑO
    // =========================

    void BasicDamage()
    {
        int spellDamage = Random.Range(100, 201);
        int totalDamage = spellDamage + playerBuffATK;

        ApplyDamageToEnemy(totalDamage);
    }

    void StrongDamage()
    {
        int spellDamage = Random.Range(250, 751);
        int totalDamage = spellDamage + playerBuffATK;

        ApplyDamageToEnemy(totalDamage);
    }

    // =========================
    // BUFFS (SOLO JUGADOR)
    // =========================

    void BasicBuff()
    {
        playerBuffATK += 10;
        playerBuffDEF += 10;
    }

    void StrongBuff()
    {
        playerBuffATK += Random.Range(25, 51);
        playerBuffDEF += Random.Range(25, 51);
    }

    // =========================
    // DEBUFFS (SOLO ENEMIGOS)
    // =========================

    void BasicDebuff()
    {
        enemyATK -= 5;
        enemyDEF -= 5;

        ClampEnemyStats();
    }

    void StrongDebuff()
    {
        enemyATK -= Random.Range(5, 11);
        enemyDEF -= Random.Range(5, 11);

        ClampEnemyStats();
    }

    void ClampEnemyStats()
    {
        enemyATK = Mathf.Max(enemyATK, 0);
        enemyDEF = Mathf.Max(enemyDEF, 0);
    }

    // =========================
    // SISTEMA DE DAÑO
    // =========================

    void ApplyDamageToEnemy(int damage)
    {
        int finalDamage = damage - enemyDEF;
        finalDamage = Mathf.Max(finalDamage, 0);

        enemyHP -= finalDamage;
        enemyHP = Mathf.Clamp(enemyHP, 0, enemyMaxHP);

        UpdateUI();
    }

    void EnemyAttack()
    {
        int finalDamage = enemyATK - (playerBaseDEF + playerBuffDEF);
        finalDamage = Mathf.Max(finalDamage, 0);

        playerHP -= finalDamage;
        playerHP = Mathf.Clamp(playerHP, 0, playerMaxHP);

        UpdateUI();
    }

    // =========================
    // UI
    // =========================

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP;
        enemyHPText.text = "Enemy HP: " + enemyHP;

        UpdateLifeSprite();
    }

    void UpdateLifeSprite()
    {
        if (lifeSprites.Length == 0) return;

        int spriteIndex = Mathf.RoundToInt((playerHP / (float)playerMaxHP) * 9);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, 9);

        lifeImage.sprite = lifeSprites[spriteIndex];
    }
}
