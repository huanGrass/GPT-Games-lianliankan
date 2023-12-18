using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class LianliankanGame : MonoBehaviour
{
    public GameObject tilePrefab; // �����Ԥ����
    public Transform boardParent; // ����ĸ�����
    public Sprite[] spList; // ���п��õķ���Sprite
    private float tileSize = 50.0f; // ����ĳߴ�

    public void InitializeBoard(LevelData levelData)
    {
        // ������з���
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        // ����shapeTemplate�б��Ϊ1�ĵ�Ԫ������
        int tileCount = 0;
        foreach (var cell in levelData.shapeTemplate)
        {
            if (cell == 1) tileCount++;
        }

        // ȷ��������ż��
        if (tileCount % 2 != 0)
        {
            Debug.LogError("��������������ż��");
            return;
        }

        // Ϊÿ�Է����������sprite
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
                    // ���ݷ���ߴ����λ��
                    float posX = j * tileSize;
                    float posY = -i * tileSize; // ע��Y���������Ҫȡ��

                    GameObject newTile = Instantiate(tilePrefab, new Vector3(posX, posY, 0), Quaternion.identity);
                    newTile.transform.SetParent(boardParent, false);
                    newTile.name = "Tile_" + i + "_" + j;

                    newTile.GetComponent<Image>().sprite = spList[tileIndices[index]];
                    index++;

                    // ������������������ã�������Ӽ����¼���
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

    // ����ָ���ؿ��������µ�ǰ�ؿ��±�
    public void LoadSelectedLevel(LevelData levelData)
    {
        // �ҵ���Ӧ�Ĺؿ��±�
        int levelIndex = levelDataAssets.FindIndex(levelAsset => levelAsset.levelData == levelData);
        if (levelIndex != -1)
        {
            InitializeBoard(levelData);
            currentLevelIndex = levelIndex; // ���µ�ǰ�ؿ��±�
        }
        else
        {
            Debug.LogError("ѡ���Ĺؿ�δ�ҵ���");
        }
    }
    // ��������...
}
