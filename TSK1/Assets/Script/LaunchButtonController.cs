using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchButtonController : MonoBehaviour
{
   
    [SerializeField] Button LaunchButton;
     public bool HitPointNotNull;

    // Update is called once per frame
    void Update()
    {
        if (HitPointNotNull)
        {
            LaunchButton.interactable = true;
        }
    }
}
