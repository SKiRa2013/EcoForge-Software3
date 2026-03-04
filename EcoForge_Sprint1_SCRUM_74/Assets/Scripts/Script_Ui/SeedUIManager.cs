using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SeedUIManager : MonoBehaviour
{
    [System.Serializable]
    public class SeedUI
    {
        public TypeSeed type;
        public Image imagen;
    }

    [SerializeField] private List<SeedUI> seedsUI;

    private Dictionary<TypeSeed, Image> seedDictionary;

    private void Awake()
    {
        seedDictionary = new Dictionary<TypeSeed, Image>();

        foreach (var seed in seedsUI)
        {
            seedDictionary.Add(seed.type, seed.imagen);
            seed.imagen.enabled = false; // Inician ocultas
        }
    }

    public void ActivarSeed(TypeSeed type)
    {
        if (seedDictionary.ContainsKey(type))
            seedDictionary[type].enabled = true;
    }

    public void DesactivarSeed(TypeSeed type)
    {
        if (seedDictionary.ContainsKey(type))
            seedDictionary[type].enabled = false;
    }
}