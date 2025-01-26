using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Bar : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
    
    }

    public void SetMaxValue(int value){
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetValue(int value){
        slider.value = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
