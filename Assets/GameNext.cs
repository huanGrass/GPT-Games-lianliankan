using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject nextLevelButton; // ����һ�ء���ť
    private int currentLevelIndex = 0; // ��ǰ�ؿ����±�
    // ������Ҫ�ı���...

    // �����������������Ƿ����з��鶼������
    // ����Ƿ����з��鶼������
    private bool AreAllTilesCleared()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile != null)
            {
                return false; // ����ҵ�δ�������ķ��飬����false
            }
        }
        return true; // ���з��鶼������
    }

    // ���ʵ���ʱ�����������������ؿ�������
    private void CheckForLevelCompletion()
    {
        if (AreAllTilesCleared())
        {
            nextLevelButton.SetActive(true); // ���з��鱻��������ʾ����һ�ء���ť
        }
    }

    // ����ָ���ؿ�
    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelDataAssets.Count)
        {
            InitializeBoard(levelDataAssets[levelIndex].levelData);
            currentLevelIndex = levelIndex;
        }
        else
        {
            Debug.LogError("�ؿ�����������Χ��");
        }
    }

    // ����ҵ������һ�ء���ťʱ����
    public void OnNextLevelClicked()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        // ����Ƿ񵽴����һ��
        if (nextLevelIndex >= levelDataAssets.Count)
        {
            nextLevelIndex = 0; // ����Ϊ��һ��
        }

        LoadLevel(nextLevelIndex);
        nextLevelButton.SetActive(false); // ���ء���һ�ء���ť
    }
    // �ڷ��������߼�֮�����
    public void OnTilesRemoved()
    {
        StartCoroutine(CheckLevelCompletionNextFrame());
    }

    private IEnumerator CheckLevelCompletionNextFrame()
    {
        yield return null; // �ȴ�ֱ����һ֡

        CheckForLevelCompletion();
    }
    // ��������...
}
