using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.05f;
    [SerializeField] private float delayBeforeStart = 1f;

    private float timer = 0f;
    private bool scrolling = false;

    void Start()
    {
        // Empieza desde arriba
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void Update()
    {
        if (!scrolling)
        {
            timer += Time.deltaTime;
            if (timer >= delayBeforeStart)
                scrolling = true;
            return;
        }

        // Baja gradualmente
        scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;

        // Cuando llega al final, se detiene
        if (scrollRect.verticalNormalizedPosition <= 0f)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            scrolling = false;
        }
    }
}