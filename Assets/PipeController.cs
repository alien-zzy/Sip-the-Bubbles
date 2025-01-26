using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    private enum Pipestatus { rotate, forward, stop, backward, suck };
    Pipestatus thispipestatus = Pipestatus.stop;
    //初始位置
    private Vector3 originPosition;
    private Quaternion originRotation;
    // 吸力大小（最大吸力）
    public float maxForceMagnitude = 30.0f;
    // 吸力减弱系数（可以控制吸力的衰减速度）
    public float forceDecayRate = 1.0f;
    //旋转速度
    public float rotationSpeed = 1.0f;
    //旋转计时器
    public float Timer = 2.5f;
    //旋转方向
    private Vector3 target = Vector3.forward;
    //前进速度
    public float translateSpeed = 5.0f;
    //后退速度（快速）
    public float fastTranSpeed = 20.0f;
    //前进后退方向
    private Vector3 govector;
    //吸取点坐标
    private Vector3 childPosition;
    //得分
    public float score;
    // 吸取计时器
    public float suckingtimer;
    //特效音
    public AudioClip soundBackground; // Assign this in the Inspector for key A
    public AudioClip soundPoints; // Assign this in the Inspector for key B
    public AudioClip soundClipC; // Assign this in the Inspector for key C

    public int maxSuckingTimer = 100;
    public int suckingTime;
    public Bar bar;



    private AudioSource audioSource;
    private void Start()
    {
        thispipestatus = Pipestatus.rotate;
        originPosition = transform.position;
        originRotation = transform.rotation;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(soundBackground);

        suckingTime = maxSuckingTimer;
        bar.SetMaxValue(maxSuckingTimer);
    }

    private void Update()
    {
        suckingTime = Mathf.RoundToInt(suckingtimer / 2 * 100);
        bar.SetValue(suckingTime);

        //获取吸取点坐标
        Transform[] trans = GetComponentsInChildren<Transform>();
        childPosition = trans[1].position;

        switch (thispipestatus)
        {
            case Pipestatus.rotate:
                SelfRotate();
                if (Input.GetKey(KeyCode.F))
                {
                    thispipestatus = Pipestatus.stop;
                }
                break;
            case Pipestatus.forward:
                SelfTranslate(govector);
                if (Input.GetKey(KeyCode.F))
                {
                    thispipestatus = Pipestatus.stop;
                }
                break;
            case Pipestatus.backward:
                //回到原点
                transform.position = originPosition;
                transform.rotation = originRotation;
                Timer = 2.5f;
                thispipestatus = Pipestatus.rotate;
                break;
            case Pipestatus.stop:
                govector = childPosition - transform.position;
                if (Input.GetKey(KeyCode.W))
                {
                    thispipestatus = Pipestatus.forward;
                }
                //按下空格键进入吸取状态
                if (Input.GetKey(KeyCode.Space))
                {
                    thispipestatus = Pipestatus.suck;
                }
                break;
            case Pipestatus.suck:
                //开始计时
                if (Input.GetKey(KeyCode.Space))
                {
                    suckingtimer -= Time.deltaTime;
                    Debug.Log($"剩余吸取时间: {suckingtimer}");
                    SuckBubble();
                }
                if (suckingtimer < 0)
                {
                    thispipestatus = Pipestatus.backward;
                    suckingtimer = 2.0f;
                }
                break;
        }
        //通过鼠标控制吸管移动
        //transform.position = GetMouseAsWorldPosition();
    }
    //沿着吸取点前进
    private void SelfTranslate(Vector3 myvector)
    {
        transform.Translate(myvector.normalized * translateSpeed * Time.deltaTime);
    }
    //绕着吸取点旋转
    private void SelfRotate()
    {
        //获取旋转中心点
        Timer -= Time.deltaTime;
        //超过一定一时间翻转方向
        if (Timer < 0)
        {
            target.z = -target.z;
            Timer = 5.0f;
        }
        transform.RotateAround(childPosition, target, rotationSpeed * Time.deltaTime);
    }


    private void SuckBubble()
    {
        // 查找所有带有"food"标签的物体
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");

        // 遍历所有找到的物体
        foreach (GameObject food in foodObjects)
        {
            // 检查物体是否有Rigidbody组件
            Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
            //获取吸入点与食物的坐标
            Vector3 foodPosition = food.GetComponent<Transform>().position;

            Vector3 forceDirection = childPosition - foodPosition;
            // 计算支点到食物的距离
            float distance = Vector3.Distance(childPosition, foodPosition);
            float forceMagnitude = maxForceMagnitude / (distance * distance + forceDecayRate);  // 加上 forceDecayRate 防止除以零和过大数值

            if (rb != null)
            {
                // 对物体施加力
                rb.AddForce(forceDirection * forceMagnitude);
                if (distance < 0.5f)  // 设定一个小的距离阈值，表示接近支点
                {
                    Destroy(food);  // 删除珍珠
                    //todo:加个特效
                    PlayDestroyEffect();
                    score += 100;  // 增加分数
                    Debug.Log($"得分: {score}");  // 打印当前分数
                }
            }
            else
            {
                Debug.LogWarning($"物体 {food.name} 没有Rigidbody组件，无法施加力。");
            }
        }
    }

    private void PlayDestroyEffect()
    {
        audioSource.PlayOneShot(soundPoints);
    }

    Vector3 GetMouseAsWorldPosition()
    {
        // 将鼠标屏幕坐标转换为世界坐标
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
