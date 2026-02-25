using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Objetivo")]
    public Transform player;

    [Header("Configuración de Seguimiento")]
    public float smoothTime = 0.2f;
    public Vector3 offset;

    [Header("Zona Muerta (Límites)")]
    public Vector2 deadZoneSize = new Vector2(2f, 2f); // Tamaño del cuadro central

    private Vector3 velocity = Vector3.zero;

    void Start() {
        offset = new Vector3(0, 0, transform.position.z);
    }

    void LateUpdate() {
        if (player == null) return;

        Vector3 currentplayerPos = transform.position - offset;

        float deltaX = player.position.x - currentplayerPos.x;
        float deltaY = player.position.y - currentplayerPos.y;

        Vector3 desiredPosition = currentplayerPos;

        if (Mathf.Abs(deltaX) > deadZoneSize.x) {
            desiredPosition.x += deltaX - (deadZoneSize.x * Mathf.Sign(deltaX));
        }

        if (Mathf.Abs(deltaY) > deadZoneSize.y) {
            desiredPosition.y += deltaY - (deadZoneSize.y * Mathf.Sign(deltaY));
        }

        Vector3 finalPosition = desiredPosition + offset;
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothTime);
    }

    // Ver zona muerta en el Editor de Unity
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position - offset;
        Gizmos.DrawWireCube(center, new Vector3(deadZoneSize.x * 2, deadZoneSize.y * 2, 0.1f));
    }
}