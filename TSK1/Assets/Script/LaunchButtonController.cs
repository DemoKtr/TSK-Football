using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchButtonController : MonoBehaviour
{
   
    [SerializeField] Button LaunchButton;
    [SerializeField] Button ResetButton;
     public bool HitPointNotNull;
    public bool Hited;

    private void Start()
    {
        ResetButton.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (HitPointNotNull)
        {
            LaunchButton.interactable = true;
        }
        if(Hited)
        {
            LaunchButton.interactable = false;
        }
    }
}
