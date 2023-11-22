using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SphereCollider triggerSphere;

    private void OnTriggerEnter(Collider other)
    {
       if(other.tag == "ball")
        {

        }
    }

    private void Triggered()
    {
        Debug.Log("SphereCollider w colliderze GoalController!");
    }
}
