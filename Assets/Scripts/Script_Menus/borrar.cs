
using UnityEngine;

public class LimpiarSaves : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < 3; i++)
            SaveSystem.BorrarSave(i);

        PlayerPrefs.DeleteAll();
        Debug.Log("Saves borrados");
    }
}