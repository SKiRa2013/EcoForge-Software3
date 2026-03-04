using UnityEngine;
using System.Collections.Generic;

public class ContenedorSeedUI : MonoBehaviour
{
    [SerializeField] private SeedUIManager uiManager;

    private Dictionary<TypeSeed, int> inventarioSeeds;

    private void Awake()
    {
        inventarioSeeds = new Dictionary<TypeSeed, int>();

        // Inicializar todas en 0
        foreach (TypeSeed tipo in System.Enum.GetValues(typeof(TypeSeed)))
        {
            inventarioSeeds.Add(tipo, 0);
        }
    }

    public void AgregarSeed(TypeSeed tipo)
    {
        inventarioSeeds[tipo]++;

        // Activar visualmente en UI
        uiManager.ActivarSeed(tipo);

        Debug.Log("Ahora tienes " + inventarioSeeds[tipo] + " semillas tipo " + tipo);
    }

    public int ObtenerCantidad(TypeSeed tipo)
    {
        return inventarioSeeds[tipo];
    }
}