using System.Collections;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    // 冰块缩小的速度（每次缩小的比例）
    public float shrinkAmount = 0.05f;

    // 冰块消失的时间
    public float shrinkInterval = 2.0f;  // 每隔多少秒缩小一次

    // 最小尺寸
    public float minSize = 0.5f;

    // 计时器
    private float timer = 0f;

    void Start()
    {
        // 启动定时器循环
        StartCoroutine(ShrinkIceBlock());
    }

    IEnumerator ShrinkIceBlock()
    {
        // 一直持续直到冰块消失
        while (transform.localScale.x > minSize)
        {
            yield return new WaitForSeconds(shrinkInterval);  // 等待一段时间（定时器）

            // 每隔 shrinkInterval 缩小一次
            transform.localScale -= new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);

            if (transform.localScale.x <= minSize)
            {
                Destroy(gameObject);  // 删除物体
            }
        }
    }
}
