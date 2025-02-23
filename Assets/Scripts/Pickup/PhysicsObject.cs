using System.Collections;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float waitOnPickup = 0.2f;
    public float breakForce = 35f;
    [HideInInspector] public bool pickedUp;
    [HideInInspector] public PlayerInteractions playerInteractions;


    private void OnCollisionEnter(Collision collision)
    {
        if (pickedUp)
            if (collision.relativeVelocity.magnitude > breakForce)
                playerInteractions.BreakConnection();
    }

    //this is used to prevent the connection from breaking when you just picked up the object as it sometimes fires a collision with the ground or whatever it is touching
    public IEnumerator PickUp()
    {
        isEnabled(false);
        yield return new WaitForSecondsRealtime(waitOnPickup);
        pickedUp = true;

        isEnabled(true);
    }

    public void isEnabled(bool value)
    {
        if (GetComponent<BoxCollider>() != null) GetComponent<BoxCollider>().enabled = value;
        foreach (Transform child in transform)
        foreach (BoxCollider comp in child.GetComponentsInChildren<BoxCollider>())
            comp.enabled = value;
    }
}