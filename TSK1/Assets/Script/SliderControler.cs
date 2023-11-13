using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text ForceValue;
    [SerializeField] Text MassValue;
    [SerializeField] Slider ForceSlider;        
    [SerializeField] Slider MassSlider;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ForceValue.text = ForceSlider.value.ToString();
        MassValue.text = MassSlider.value.ToString();
    }
}
