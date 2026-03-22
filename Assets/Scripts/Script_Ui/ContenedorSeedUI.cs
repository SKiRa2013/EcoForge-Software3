using UnityEngine;
using System.Collections.Generic;

public class ContenedorSeedUI : MonoBehaviour
{
    [SerializeField] private SeedUIManager uiManager;

    [SerializeField] private int maxStack = 6;

    private Dictionary<TypeSeed, int> inventarioSeeds;

    private void Awake()
    {
        inventarioSeeds = new Dictionary<TypeSeed, int>();

        foreach (TypeSeed tipo in System.Enum.GetValues(typeof(TypeSeed)))
        {
            inventarioSeeds.Add(tipo, 0);
        }
    }

    public void AgregarSeed(TypeSeed tipo)
    {
        if (inventarioSeeds[tipo] >= maxStack)
        {
            Debug.Log("El contenedor de semillas tipo " + tipo + " está lleno.");
            return;
        }

        inventarioSeeds[tipo]++;

        // Actualiza la UI con la cantidad actual
        uiManager.ActualizarStack(tipo, inventarioSeeds[tipo]);

        Debug.Log("Semillas tipo " + tipo + ": " + inventarioSeeds[tipo]);
    }

    public void UsarSeed(TypeSeed tipo)
    {
        if (inventarioSeeds[tipo] <= 0)
        {
            Debug.Log("No hay semillas tipo " + tipo);
            return;
        }

        inventarioSeeds[tipo]--;

        // Actualiza la UI
        uiManager.ActualizarStack(tipo, inventarioSeeds[tipo]);

        Debug.Log("Semillas restantes tipo " + tipo + ": " + inventarioSeeds[tipo]);
    }

    public int ObtenerCantidad(TypeSeed tipo)
    {
        return inventarioSeeds[tipo];
    }
}