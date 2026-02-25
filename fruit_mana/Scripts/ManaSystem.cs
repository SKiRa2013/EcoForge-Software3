using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaSystem : MonoBehaviour
{
    [Header("Configuración")]
    public int maxMana = 1000;
    public int currentMana;

    [Header("Referencias UI")]
    public Image manaImage;
    public TextMeshProUGUI manaText;

    [Header("Sprites (posion_0 a posion_10)")]
    public Sprite[] manaSprites; // 11 sprites

    void Start()
    {
        currentMana = maxMana;
        UpdateUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            PlantRedTree();

        if (Input.GetKeyDown(KeyCode.E))
            EatGoldenFruit();
    }
    // ==================================================
    // GASTAR MANÁ PARA ÁRBOLES
    // ==================================================

    public bool PlantBasicTree() => SpendMana(1);
    public bool PlantGreenTree() => SpendMana(2);
    public bool PlantBlueTree() => SpendMana(4);
    public bool PlantRedTree() => SpendMana(10);
    public bool PlantGoldenTree() => SpendMana(20);

    // ==================================================
    // REGENERACIÓN POR FRUTAS
    // ==================================================

    public void EatBasicFruit() => AddMana(2);
    public void EatGreenFruit() => AddMana(4);
    public void EatBlueFruit() => AddMana(8);
    public void EatRedFruit() => AddMana(20);
    public void EatGoldenFruit() => AddMana(40);


    bool SpendMana(int amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;
        UpdateUI();
        return true;
    }

    void AddMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateUI();
    }

    void UpdateUI()
    {
        // Actualizar texto
        manaText.text = currentMana.ToString();

        // Calcular índice 0–9
        float percent = (float)currentMana / maxMana;
        int index = Mathf.FloorToInt(percent * 9);

        index = Mathf.Clamp(index, 0, 9);

        manaImage.sprite = manaSprites[index];
    }
}