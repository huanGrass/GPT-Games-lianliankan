using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject tilePrefab; // 方块的预制体
    public Transform boardParent; // 方块的父对象
    public Sprite[] spList; // 所有可用的方块Sprite
    private float tileSize = 50.0f; // 方块的尺寸

    public void InitializeBoard(LevelData levelData)
    {
        // 清除现有方块
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        // 计算shapeTemplate中标记为1的单元格数量
        int tileCount = 0;
        foreach (var cell in levelData.shapeTemplate)
        {
            if (cell == 1) tileCount++;
        }

        // 确保数量是偶数
        if (tileCount % 2 != 0)
        {
            Debug.LogError("方块数量必须是偶数");
            return;
        }

        // 为每对方块随机分配sprite
        List<int> tileIndices = new List<int>();
        for (int i = 0; i < tileCount / 2; i++)
        {
            int spriteIndex = i % spList.Length;
            tileIndices.Add(spriteIndex);
            tileIndices.Add(spriteIndex);
        }
        Shuffle(tileIndices);
        int[,] shape = levelData.GetShapeTemplateAs2D();
        int index = 0;
        tiles = new GameObject[levelData.rows + 2, levelData.cols + 2];
        for (int i = 1; i <= levelData.rows; i++)
        {
            for (int j = 1; j <= levelData.cols; j++)
            {
                if (shape[i-1, j-1] == 1)
                {
                    // 根据方块尺寸计算位置
                    float posX = j * tileSize;
                    float posY = -i * tileSize; // 注意Y坐标可能需要取反

                    GameObject newTile = Instantiate(tilePrefab, new Vector3(posX, posY, 0), Quaternion.identity);
                    newTile.transform.SetParent(boardParent, false);
                    newTile.name = "Tile_" + i + "_" + j;

                    newTile.GetComponent<Image>().sprite = spList[tileIndices[index]];
                    index++;

                    // 可以添加其他属性设置，例如添加监听事件等
                    int x = i, y = j;
                    newTile.GetComponent<Button>().onClick.AddListener(() => TileClicked(x, y));

                    tiles[i, j] = newTile;
                }
            }
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // 加载指定关卡，并更新当前关卡下标
    public void LoadSelectedLevel(LevelData levelData)
    {
        // 找到对应的关卡下标
        int levelIndex = levelDataAssets.FindIndex(levelAsset => levelAsset.levelData == levelData);
        if (levelIndex != -1)
        {
            InitializeBoard(levelData);
            currentLevelIndex = levelIndex; // 更新当前关卡下标
        }
        else
        {
            Debug.LogError("选定的关卡未找到！");
        }
    }
    // 其他方法...
}
