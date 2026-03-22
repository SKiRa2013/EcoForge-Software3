using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaSystem : MonoBehaviour
{
    [Header("Configuración")]
    public int maxMana = 1000;
    public int currentMana;

    [Header("UI")]
    public Image manaImage;
    public TextMeshProUGUI manaText;

    [Header("Sprites de barra de mana")]
    public Sprite[] manaSprites;

    void Start()
    {
        currentMana = maxMana;
        UpdateUI();
    }

    // ---------- GASTAR MANÁ ----------

    public bool SpendMana(int amount)
    {
        if (currentMana < amount)
        {
            Debug.Log("No hay suficiente mana");
            return false;
        }

        currentMana -= amount;

        UpdateUI();
        return true;
    }

    // ---------- RECUPERAR MANÁ ----------

    public void AddMana(int amount)
    {
        currentMana += amount;

        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        UpdateUI();
    }

    // ---------- ACTUALIZAR UI ----------

    void UpdateUI()
    {
        if (manaText != null)
            manaText.text = currentMana.ToString();

        float percent = (float)currentMana / maxMana;

        int index = Mathf.FloorToInt(percent * 9);
        index = Mathf.Clamp(index, 0, 9);

        if (manaImage != null && manaSprites.Length > index)
            manaImage.sprite = manaSprites[index];
    }
} 
