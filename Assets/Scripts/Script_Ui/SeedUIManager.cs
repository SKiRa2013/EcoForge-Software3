using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SeedUIManager : MonoBehaviour
{
    [System.Serializable]
    public class SeedStack
    {
        public TypeSeed type;
        public Image[] iconos; // tamaño 6
    }

    [SerializeField] private SeedStack[] stacks;

    private Dictionary<TypeSeed, Image[]> stacksDictionary;

    private void Awake()
    {
        stacksDictionary = new Dictionary<TypeSeed, Image[]>();

        foreach (SeedStack stack in stacks)
        {
            stacksDictionary.Add(stack.type, stack.iconos);

            // 🔹 OCULTAR TODOS LOS ICONOS AL INICIO
            for (int i = 0; i < stack.iconos.Length; i++)
            {
                stack.iconos[i].enabled = false;
            }
        }
    }

    public void ActualizarStack(TypeSeed tipo, int cantidad)
    {
        if (!stacksDictionary.ContainsKey(tipo))
            return;

        Image[] iconos = stacksDictionary[tipo];

        for (int i = 0; i < iconos.Length; i++)
        {
            iconos[i].enabled = i < cantidad;
        }
    }
}