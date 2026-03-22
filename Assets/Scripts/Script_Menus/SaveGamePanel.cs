using UnityEngine;

public class SaveGamePanel : MonoBehaviour
{
    public SaveSlotUI[] slots;

    void OnEnable()
    {
        ActualizarSlots();
    }

    public void ActualizarSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (SaveSystem.ExisteSave(i))
            {
                var data = SaveSystem.CargarInfo(i);
                slots[i].SetData(data.nombre, data.fecha, data.progreso);
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }
}