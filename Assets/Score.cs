using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    // 引用 UI 中的 TextMeshProUGUI 组件
    [SerializeField] TextMeshProUGUI scoreText;

    // 得分变量（可以是 public 以便通过其他脚本访问）
    public int score = 0;

    // 更新分数显示的方法
    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreText.text = score.ToString(); // 更新文本

        // Debug.Log("Score updated to: " + scoreText.text);
    }
}
