using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MirrorRoom : MonoBehaviour
{
    public GameObject Mirror;
    private int count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            count++;
            Mirror.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3) count--;
        if (count == 0) Mirror.SetActive(false);
    }
}