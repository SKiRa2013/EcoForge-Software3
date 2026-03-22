using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    static string GetRuta(int slot)
    {
        return Application.persistentDataPath + "/save_slot_" + slot + ".json";
    }

    public static bool ExisteSave(int slot)
    {
        return File.Exists(GetRuta(slot));
    }

    // ── Verificar si un nombre ya existe en otro slot ──
    public static bool NombreExiste(string nombre, int slotExcluir = -1)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == slotExcluir) continue;
            if (!ExisteSave(i)) continue;

            var data = CargarJuego(i);
            if (data != null && data.nombre.ToLower().Trim() == nombre.ToLower().Trim())
                return true;
        }
        return false;
    }

    // ── Obtener primer slot vacío ──
    public static int ObtenerSlotLibre()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!ExisteSave(i)) return i;
        }
        return -1; // No hay slots libres
    }

    public static void GuardarJuego(int slot, Transform player, string nombre = "")
    {
        GameData data = new GameData();

        data.nombre   = string.IsNullOrEmpty(nombre) ? "Partida " + (slot + 1) : nombre;
        data.escena   = SceneManager.GetActiveScene().name;
        data.posX     = player.position.x;
        data.posY     = player.position.y;
        data.fecha    = System.DateTime.Now.ToString("dd/MM/yyyy  HH:mm");
        data.progreso = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetRuta(slot), json);

        Debug.Log("Juego guardado en slot " + slot + " como: " + data.nombre);
    }

    // ── Crear partida nueva con nombre ──
    public static void CrearPartida(int slot, string nombre)
    {
        GameData data = new GameData();

        data.nombre   = nombre;
        data.escena   = "Juego";
        data.posX     = 0;
        data.posY     = 0;
        data.fecha    = System.DateTime.Now.ToString("dd/MM/yyyy  HH:mm");
        data.progreso = "Inicio";

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetRuta(slot), json);

        Debug.Log("Partida creada en slot " + slot + ": " + nombre);
    }

    // ── Renombrar partida existente ──
    public static bool RenombrarPartida(int slot, string nuevoNombre)
    {
        if (!ExisteSave(slot)) return false;

        var data = CargarJuego(slot);
        if (data == null) return false;

        data.nombre = nuevoNombre;
        data.fecha  = System.DateTime.Now.ToString("dd/MM/yyyy  HH:mm");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetRuta(slot), json);

        return true;
    }

    public static GameData CargarJuego(int slot)
    {
        string ruta = GetRuta(slot);

        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            return JsonUtility.FromJson<GameData>(json);
        }

        return null;
    }

    public static GameData CargarInfo(int slot)
    {
        return CargarJuego(slot);
    }

    public static void BorrarSave(int slot)
    {
        string ruta = GetRuta(slot);

        if (File.Exists(ruta))
        {
            File.Delete(ruta);
            Debug.Log("Save borrado en slot " + slot);
        }
    }
}


