using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float remainingTime;

    // Update is called once per frame
    void Update()
    {
        remainingTime -= Time.deltaTime;

        if(remainingTime >0){
            remainingTime -= Time.deltaTime;
        }else if(remainingTime < 0){
            remainingTime = 0;
            timerText.color = Color.red;
        }
        int minutes = Mathf.FloorToInt(remainingTime /60);
        int seconds = Mathf.FloorToInt(remainingTime %60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }
    
}
