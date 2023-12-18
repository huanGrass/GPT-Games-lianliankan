using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject nextLevelButton; // “下一关”按钮
    private int currentLevelIndex = 0; // 当前关卡的下标
    // 其他需要的变量...

    // 调用这个方法来检查是否所有方块都被消除
    // 检查是否所有方块都被消除
    private bool AreAllTilesCleared()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile != null)
            {
                return false; // 如果找到未被消除的方块，返回false
            }
        }
        return true; // 所有方块都被消除
    }

    // 在适当的时候调用这个方法来检查关卡完成情况
    private void CheckForLevelCompletion()
    {
        if (AreAllTilesCleared())
        {
            nextLevelButton.SetActive(true); // 所有方块被消除，显示“下一关”按钮
        }
    }

    // 加载指定关卡
    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelDataAssets.Count)
        {
            InitializeBoard(levelDataAssets[levelIndex].levelData);
            currentLevelIndex = levelIndex;
        }
        else
        {
            Debug.LogError("关卡索引超出范围！");
        }
    }

    // 当玩家点击“下一关”按钮时调用
    public void OnNextLevelClicked()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        // 检查是否到达最后一关
        if (nextLevelIndex >= levelDataAssets.Count)
        {
            nextLevelIndex = 0; // 重置为第一关
        }

        LoadLevel(nextLevelIndex);
        nextLevelButton.SetActive(false); // 隐藏“下一关”按钮
    }
    // 在方块消除逻辑之后调用
    public void OnTilesRemoved()
    {
        StartCoroutine(CheckLevelCompletionNextFrame());
    }

    private IEnumerator CheckLevelCompletionNextFrame()
    {
        yield return null; // 等待直到下一帧

        CheckForLevelCompletion();
    }
    // 其他方法...
}
