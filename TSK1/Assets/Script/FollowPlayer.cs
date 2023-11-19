using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;  // Przypisz obiekt aktora (np. gracza) tutaj
    public float smoothSpeed = 0.125f;  // Dostosuj pr�dko�� �ledzenia kamery

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + new Vector3(0, 0.3f, -1.5f);  // Dostosuj warto�� Z, aby ustawi� odleg�o�� kamery od aktora
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
