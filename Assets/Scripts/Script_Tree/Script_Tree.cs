using UnityEngine;

public class Planta : MonoBehaviour
{
    [SerializeField] private float tiempoCrecimiento = 12f;

    [SerializeField] private Vector3 escalaInicial = new Vector3(0.5f, 0.5f, 1f);
    [SerializeField] private Vector3 escalaFinal = new Vector3(1.5f, 1.5f, 1f);

    private float timer;
    private bool haCrecido;

    private void Start()
    {
        transform.localScale = escalaInicial;
    }

    private void Update()
    {
        if (haCrecido)
            return;

        timer += Time.deltaTime;

        float progreso = Mathf.Clamp01(timer / tiempoCrecimiento);

        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);

        if (progreso >= 1f)
        {
            haCrecido = true;
            PlantaLista();
        }
    }

    private void PlantaLista()
    {
        Debug.Log("La planta terminó de crecer.");
    }
}