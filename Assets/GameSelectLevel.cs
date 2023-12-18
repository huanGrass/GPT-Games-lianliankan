using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject levelButtonPrefab; // 关卡按钮的预制体
    public Transform levelButtonParent;  // 关卡按钮的父对象
    public List<LevelDataAsset> levelDataAssets; // 所有关卡数据

    void Start()
    {
        backgroundMusicSource.Play();
        GenerateLevelButtons();
        // 为“下一关”按钮添加点击事件监听器
        Button nextLevelButtonComponent = nextLevelButton.GetComponent<Button>();
        if (nextLevelButtonComponent != null)
        {
            nextLevelButtonComponent.onClick.AddListener(OnNextLevelClicked);
        }
        else
        {
            Debug.LogError("下一关按钮上未找到Button组件！");
        }
    }

    void GenerateLevelButtons()
    {
        foreach (LevelDataAsset levelDataAsset in levelDataAssets)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonParent);
            buttonObj.GetComponentInChildren<Text>().text = levelDataAsset.levelData.levelName; // 假设您的按钮上有一个Text组件显示关卡名称

            // 添加按钮的点击事件
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(levelDataAsset.levelData));
        }
    }

    void OnLevelButtonClicked(LevelData levelData)
    {
        levelButtonParent.gameObject.SetActive(false);
        // 现有的加载关卡的逻辑
        LoadSelectedLevel(levelData);
    }

    // 现有的InitializeBoard方法...
}
