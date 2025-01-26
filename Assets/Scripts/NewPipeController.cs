using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


public class NewPipeController : MonoBehaviour
{
    private enum Pipestatus { rotate, forward, stop, backward, suck };
    Pipestatus thispipestatus = Pipestatus.stop;
    //初始位置
    private Vector3 originPosition;
    private Quaternion originRotation;
    private Vector3 PipeoriginPosition;
    private Quaternion PipeoriginRotation;

    public TextMeshProUGUI scoreText;
    public Score scoreDisplay;

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
    //吸取点坐标
    private Vector3 childPosition;
    //得分
    public int score = 0;
    // 吸取计时器
    public float suckingtimer;
    //吸管
    public GameObject pipe;
    //吸取点
    public GameObject point;
    //特效音
    public AudioClip soundBackground; // Assign this in the Inspector for key A
    public AudioClip soundPoints; // Assign this in the Inspector for key B
    public int maxSuckingTimer = 100;
    public int suckingTime;
    public Bar bar;
    public Animator handanimator;



    private AudioSource audioSource;
    private void Start()
    {
        thispipestatus = Pipestatus.rotate;
        PipeoriginPosition = pipe.transform.position;
        PipeoriginRotation = pipe.transform.rotation;
        originPosition = transform.position;
        originRotation = transform.rotation;
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(soundBackground);

        suckingTime = maxSuckingTimer;
        bar.SetMaxValue(maxSuckingTimer);
    }

    private void Update()
    {
        //体力条
        suckingTime = Mathf.RoundToInt(suckingtimer / 2 * 100);
        bar.SetValue(suckingTime);
        //切换管子状态
        switch (thispipestatus)
        {
            case Pipestatus.rotate:
                SelfRotate();
                if (Input.GetKey(KeyCode.F))
                {
                    thispipestatus = Pipestatus.forward;
                }
                break;
            case Pipestatus.forward:
                pipe.transform.position = Vector3.MoveTowards(pipe.transform.position, point.transform.position, translateSpeed * Time.deltaTime);
                handanimator.SetBool("Drop", true);
                if (Input.GetKey(KeyCode.Q))
                {
                    thispipestatus = Pipestatus.stop;
                    handanimator.SetBool("Drop", false);
                }
                if (Input.GetKey(KeyCode.Escape))
                {
                    thispipestatus = Pipestatus.backward;
                    handanimator.SetBool("Drop", false);
                }
                break;
            case Pipestatus.backward:
                //回到原点
                transform.rotation = originRotation;
                transform.position = originPosition;
                pipe.transform.rotation = PipeoriginRotation;
                pipe.transform.position = PipeoriginPosition;
                Timer = 2.5f;
                thispipestatus = Pipestatus.rotate;
                break;
            case Pipestatus.stop:
                //按下空格键进入吸取状态
                if (Input.GetKey(KeyCode.Space))
                {
                    thispipestatus = Pipestatus.suck;
                }
                if (Input.GetKey(KeyCode.Escape))
                {
                    thispipestatus = Pipestatus.backward;
                }
                break;
            case Pipestatus.suck:
                //开始计时
                if (Input.GetKey(KeyCode.Space))
                {
                    suckingtimer -= Time.deltaTime;
                    SuckBubble();
                }
                if (suckingtimer < 0)
                {
                    thispipestatus = Pipestatus.backward;
                    suckingtimer = 2.0f;
                }
                if (Input.GetKey(KeyCode.Escape))
                {
                    thispipestatus = Pipestatus.backward;
                }
                break;
        }
        //通过鼠标控制吸管移动
        //transform.position = GetMouseAsWorldPosition();

        if (scoreDisplay != null)
        {
            scoreDisplay.UpdateScore(score);  // 传递更新后的分数
        }
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
            Timer = 4.5f;
        }
        transform.RotateAround(point.transform.position, target, rotationSpeed * Time.deltaTime);
    }


    private void SuckBubble()
    {
        // 查找所有带有"food"标签的物体
        GameObject[] yeguoObjects = GameObject.FindGameObjectsWithTag("Yeguo");
        GameObject[] bubbleObjects = GameObject.FindGameObjectsWithTag("Bubble");
        GameObject[] iceObjects = GameObject.FindGameObjectsWithTag("Ice");
        GameObject[] yuyuanObjects = GameObject.FindGameObjectsWithTag("Yuyuan");

        ProcessFood(yuyuanObjects, 5);     // yuyuan食物，得分5
        ProcessFood(yeguoObjects, 10);  // yeguo食物，得分10
        ProcessFood(bubbleObjects, 15);  // bubble食物，得分5
        ProcessFood(iceObjects, -10);     // ice食物，得分2

        // 遍历所有找到的物体
        // foreach (GameObject food in foodObjects)
        // {
        //     // 检查物体是否有Rigidbody组件
        //     Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        //     //获取吸入点与食物的坐标
        //     Vector3 foodPosition = food.GetComponent<Transform>().position;

        //     Vector3 forceDirection = point.transform.position - foodPosition;
        //     // 计算支点到食物的距离
        //     float distance = Vector3.Distance(point.transform.position, foodPosition);
        //     float forceMagnitude = maxForceMagnitude / (distance * distance + forceDecayRate);  // 加上 forceDecayRate 防止除以零和过大数值

        //     if (rb != null)
        //     {
        //         // 对物体施加力
        //         rb.AddForce(forceDirection * forceMagnitude);
        //         if (distance < 0.5f)  // 设定一个小的距离阈值，表示接近支点
        //         {
        //             //判断加分
        //             Destroy(food);  // 删除珍珠
        //             if(Destory(yuyuanObjects)){
        //                 PlayDestroyEffect();
        //                 Debug.Log($"得分:{score}");
        //             }

        //             //todo:加个特效
        //             PlayDestroyEffect();

        //             Debug.Log($"得分: {score}");  // 打印当前分数
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"物体 {food.name} 没有Rigidbody组件，无法施加力。");
        //     }
        // }
    }

    private void ProcessFood(GameObject[] foodObjects, int scoreValue)
    {
    foreach (GameObject food in foodObjects)
    {
        // 输出食物的标签，检查是否正确
        // Debug.Log($"正在处理食物: {food.name}，标签: {food.tag}");

        // 检查物体是否有Rigidbody组件
        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        // 获取吸入点与食物的坐标
        Vector3 foodPosition = food.transform.position;

        Vector3 forceDirection = point.transform.position - foodPosition;
        // 计算支点到食物的距离
        float distance = Vector3.Distance(point.transform.position, foodPosition);
        float forceMagnitude = maxForceMagnitude / (distance * distance + forceDecayRate);

        if (rb != null)
        {
            // 对物体施加力
            rb.AddForce(forceDirection * forceMagnitude);

            if (distance < 0.5f)  // 设定一个小的距离阈值，表示接近支点
            {
                // 增加或减少对应类型食物的分数
                score += scoreValue;
                if(score <0){
                    score = 0 ;
                }
              
                

                // 输出分数更新信息
                // Debug.Log($"得分: {score}");  // 打印当前分数

                // 删除食物物体
                Destroy(food);
                PlayDestroyEffect();  // 播放销毁特效
                scoreDisplay.UpdateScore(score);
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
        if (soundPoints != null)
        {
        audioSource.PlayOneShot(soundPoints);
        Debug.Log("播放音效: soundPoints");
        }
        else
        {
        Debug.LogWarning("音效 soundPoints 为空，无法播放。");
        }
    }

}
