using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

public class Skill : MonoBehaviour
{

    public float maxAngle = 10f; // Maximum angle in degrees
    public float speed = 5f; // Speed of the oscillation
    public float upForce = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            float zRotation = maxAngle * Mathf.Sin(Time.time * speed);

            // Apply the rotation to the object
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

            
            GameObject[] foodObjects1 = GameObject.FindGameObjectsWithTag("Yeguo");
            GameObject[] foodObjects2 = GameObject.FindGameObjectsWithTag("Yuyuan");
            GameObject[] foodObjects3 = GameObject.FindGameObjectsWithTag("Bubble");
            GameObject[] allFoodObjects = new GameObject[foodObjects1.Length + foodObjects2.Length + foodObjects3.Length];
            foodObjects1.CopyTo(allFoodObjects, 0);
            foodObjects2.CopyTo(allFoodObjects, foodObjects1.Length);
            foodObjects3.CopyTo(allFoodObjects, foodObjects1.Length + foodObjects2.Length);
            // 遍历所有找到的物体
            foreach (GameObject food in allFoodObjects)
            {
                Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
                rb.AddForce(Vector2.up * upForce);
            }

        }
    }

}
