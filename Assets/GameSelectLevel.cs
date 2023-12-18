using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject levelButtonPrefab; // �ؿ���ť��Ԥ����
    public Transform levelButtonParent;  // �ؿ���ť�ĸ�����
    public List<LevelDataAsset> levelDataAssets; // ���йؿ�����

    void Start()
    {
        backgroundMusicSource.Play();
        GenerateLevelButtons();
        // Ϊ����һ�ء���ť��ӵ���¼�������
        Button nextLevelButtonComponent = nextLevelButton.GetComponent<Button>();
        if (nextLevelButtonComponent != null)
        {
            nextLevelButtonComponent.onClick.AddListener(OnNextLevelClicked);
        }
        else
        {
            Debug.LogError("��һ�ذ�ť��δ�ҵ�Button�����");
        }
    }

    void GenerateLevelButtons()
    {
        foreach (LevelDataAsset levelDataAsset in levelDataAssets)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonParent);
            buttonObj.GetComponentInChildren<Text>().text = levelDataAsset.levelData.levelName; // �������İ�ť����һ��Text�����ʾ�ؿ�����

            // ��Ӱ�ť�ĵ���¼�
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(levelDataAsset.levelData));
        }
    }

    void OnLevelButtonClicked(LevelData levelData)
    {
        levelButtonParent.gameObject.SetActive(false);
        // ���еļ��عؿ����߼�
        LoadSelectedLevel(levelData);
    }

    // ���е�InitializeBoard����...
}
