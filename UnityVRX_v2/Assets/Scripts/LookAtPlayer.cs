using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        Vector3 direction = player.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.rotation = rotation * Quaternion.Euler(0, 90, 0);
    }
}