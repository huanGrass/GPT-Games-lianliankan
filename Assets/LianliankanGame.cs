using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class LianliankanGame : MonoBehaviour
{
    private GameObject[,] tiles;
    public AudioSource backgroundMusicSource; // 背景音乐的AudioSource
    public AudioSource clickSoundEffectSource; // 点击音效的AudioSource
    public AudioSource eliminationSoundEffectSource; // 消除音效的AudioSource

    private GameObject selectedTile1 = null;
    private GameObject selectedTile2 = null;

    private GameObject currentSelectionEffect;
    public GameObject selectionEffectPrefab;

    void TileClicked(int x, int y)
    {
        clickSoundEffectSource.Play();
        GameObject clickedTile = tiles[x, y];
        if (clickedTile == selectedTile1)
        {
            selectedTile1 = null;
            // 可以添加取消选择的视觉反馈
            HideSelectionEffect(); // 隐藏之前的选择效果
            return;
        }
        if (selectedTile1 == null)
        {
            selectedTile1 = clickedTile;
            ShowSelectionEffect(clickedTile);
        }
        else
        {
            selectedTile2 = clickedTile;
            HideSelectionEffect(); // 隐藏之前的选择效果
            CheckAndRemoveTiles();
        }
    }

    void ShowSelectionEffect(GameObject tile)
    {
        if (currentSelectionEffect != null)
        {
            HideSelectionEffect();
        }

        currentSelectionEffect = Instantiate(selectionEffectPrefab, tile.transform.position, Quaternion.identity);
        currentSelectionEffect.transform.SetParent(tile.transform); // 不传入 false 作为第二个参数
    }


    void HideSelectionEffect()
    {
        if (currentSelectionEffect != null)
        {
            Destroy(currentSelectionEffect);
            currentSelectionEffect = null;
        }
    }


    void CheckAndRemoveTiles()
    {
        if (selectedTile1 != null && selectedTile2 != null)
        {
            // 检查是否有相同的图案
            if (selectedTile1.GetComponent<Image>().sprite == selectedTile2.GetComponent<Image>().sprite)
            {
                // 检查两个方块是否可以连接，并获取拐角点
                List<Vector2> cornerPoints = CanBeConnected(selectedTile1, selectedTile2);
                if (cornerPoints != null)
                {
                    // 如果可以连接，绘制连接线段
                    DrawLinesForMatch(selectedTile1, selectedTile2, cornerPoints);

                    // 移除方块
                    Destroy(selectedTile1);
                    Destroy(selectedTile2);
                    OnTilesRemoved();
                    eliminationSoundEffectSource.Play();
                }
            }

            // 重置选择
            selectedTile1 = null;
            selectedTile2 = null;
        }
    }

    public void OnReturnButtonClicked()
    {
        SceneManager.LoadScene("MainMenu"); // 加载开始界面的场景名
    }

    List<Vector2> CanBeConnected(GameObject tile1, GameObject tile2)
    {
        // 获取方块的行列位置
        Vector2 pos1 = GetPositionInGrid(tile1);
        Vector2 pos2 = GetPositionInGrid(tile2);

        // 检查是否在同一行或同一列
        if (pos1.x == pos2.x || pos1.y == pos2.y)
        {
            if (IsPathClear(pos1, pos2))
            {
                return new List<Vector2>(); // 直线连接，没有拐角
            }
        }

        // 检查通过一个转角连接
        List<Vector2> oneCornerConnection = IsOneCornerConnection(pos1, pos2);
        if (oneCornerConnection != null)
        {
            return oneCornerConnection;
        }

        // 检查通过两个转角连接
        List<Vector2> twoCornerConnection = IsTwoCornerConnection(pos1, pos2);
        return twoCornerConnection; // 可能是 null，如果没有两个转角连接
    }


    bool IsPathClear(Vector2 pos1, Vector2 pos2)
    {
        if (pos1.x == pos2.x)
        {
            // 同一列
            int startY = (int)Mathf.Min(pos1.y, pos2.y);
            int endY = (int)Mathf.Max(pos1.y, pos2.y);
            for (int y = startY + 1; y < endY; y++)
            {
                if (tiles[(int)pos1.x, y] != null)
                {
                    // 路径被阻挡
                    return false;
                }
            }
        }
        else if (pos1.y == pos2.y)
        {
            // 同一行
            int startX = (int)Mathf.Min(pos1.x, pos2.x);
            int endX = (int)Mathf.Max(pos1.x, pos2.x);
            for (int x = startX + 1; x < endX; x++)
            {
                if (tiles[x, (int)pos1.y] != null)
                {
                    // 路径被阻挡
                    return false;
                }
            }
        }
        else
        {
            // 不在同一行或同一列
            return false;
        }

        // 路径清晰
        return true;
    }


    List<Vector2> IsOneCornerConnection(Vector2 pos1, Vector2 pos2)
    {
        // 检查第一个可能的转角点
        Vector2 corner1 = new Vector2(pos1.x, pos2.y);
        if (tiles[(int)corner1.x, (int)corner1.y] == null) // 确保转角点是空的
        {
            if (IsPathClear(pos1, corner1) && IsPathClear(corner1, pos2))
            {
                return new List<Vector2> { corner1 }; // 返回包含一个拐角点的列表
            }
        }

        // 检查第二个可能的转角点
        Vector2 corner2 = new Vector2(pos2.x, pos1.y);
        if (tiles[(int)corner2.x, (int)corner2.y] == null) // 确保转角点是空的
        {
            if (IsPathClear(pos1, corner2) && IsPathClear(corner2, pos2))
            {
                return new List<Vector2> { corner2 }; // 返回包含一个拐角点的列表
            }
        }

        return null; // 如果没有拐角连接，返回null
    }

    List<Vector2> IsTwoCornerConnection(Vector2 pos1, Vector2 pos2)
    {
        // 遍历与pos1或pos2同行或同列的空格子
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            if (tiles[x, (int)pos1.y] == null && IsPathClear(pos1, new Vector2(x, pos1.y)))
            {
                Vector2 corner1 = new Vector2(x, pos1.y);
                List<Vector2> corners = IsOneCornerConnection(corner1, pos2);
                if (corners != null)
                {
                    corners.Insert(0, corner1); // 将第一个拐角点添加到列表开始位置
                    return corners;
                }
            }
            if (tiles[x, (int)pos2.y] == null && IsPathClear(pos2, new Vector2(x, pos2.y)))
            {
                Vector2 corner1 = new Vector2(x, pos2.y);
                List<Vector2> corners = IsOneCornerConnection(pos1, corner1);
                if (corners != null)
                {
                    corners.Add(corner1); // 将第一个拐角点添加到列表末尾
                    return corners;
                }
            }
        }

        for (int y = 0; y < tiles.GetLength(1); y++)
        {
            if (tiles[(int)pos1.x, y] == null && IsPathClear(pos1, new Vector2(pos1.x, y)))
            {
                Vector2 corner1 = new Vector2(pos1.x, y);
                List<Vector2> corners = IsOneCornerConnection(corner1, pos2);
                if (corners != null)
                {
                    corners.Insert(0, corner1);
                    return corners;
                }
            }
            if (tiles[(int)pos2.x, y] == null && IsPathClear(pos2, new Vector2(pos2.x, y)))
            {
                Vector2 corner1 = new Vector2(pos2.x, y);
                List<Vector2> corners = IsOneCornerConnection(pos1, corner1);
                if (corners != null)
                {
                    corners.Add(corner1);
                    return corners;
                }
            }
        }

        return null; // 如果没有两个拐角连接，返回null
    }


    Vector2 GetPositionInGrid(GameObject tile)
    {
        string name = tile.name;
        // 假设名称格式为 "Tile_x_y"，其中x和y是行列位置
        string[] parts = name.Split('_');
        if (parts.Length == 3)
        {
            int x, y;
            if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
            {
                return new Vector2(x, y);
            }
        }

        Debug.LogError("无法从方块名称解析位置: " + name);
        return new Vector2(-1, -1);
    }




    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 检测点击并处理选择逻辑
        }
    }

    bool CheckMatch(GameObject tile1, GameObject tile2)
    {
        // 检查是否可以连接tile1和tile2
        return false;
    }

    void RemoveTiles(GameObject tile1, GameObject tile2)
    {
        // 移除匹配的方块
    }
    public GameObject lineImagePrefab; // 在编辑器中设置这个变量

    void DrawLinesForMatch(GameObject tile1, GameObject tile2, List<Vector2> cornerPoints)
    {
        Vector2 startPos = tile1.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = tile2.GetComponent<RectTransform>().anchoredPosition;

        // 转换拐角点坐标
        List<Vector2> uiCornerPoints = cornerPoints.Select(p => GridIndexToUIPosition((int)p.x, (int)p.y)).ToList();

        // 包含所有点的列表（起点、中间拐角点的UI位置、终点）
        List<Vector2> points = new List<Vector2> { startPos };
        points.AddRange(uiCornerPoints);
        points.Add(endPos);

        // 为每段路径绘制一条线
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLineBetweenPoints(points[i], points[i + 1]);
        }
    }
    Vector2 GridIndexToUIPosition(int x, int y)
    {
        float tileWidth = 50f;
        float tileHeight = 50f;
        // 由于tiles的第一个索引对应的是y坐标，第二个索引对应的是x坐标，我们需要调换x和y的位置
        return new Vector2(y * tileWidth, -x * tileHeight);
    }


    void DrawLineBetweenPoints(Vector2 start, Vector2 end)
    {
        GameObject lineObj = Instantiate(lineImagePrefab, transform);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        // 设置线段的位置
        lineRect.anchoredPosition = (start + end) / 2;

        // 确定是水平还是垂直线段，并设置长度
        if (start.x == end.x) // 垂直线段
        {
            float length = Mathf.Abs(end.y - start.y);
            lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, length);
        }
        else // 水平线段
        {
            float length = Mathf.Abs(end.x - start.x);
            lineRect.sizeDelta = new Vector2(length, lineRect.sizeDelta.y);
        }

        // 设置定时器销毁线段
        Destroy(lineObj, 2f); // 例如2秒后销毁
    }


    // 其他辅助方法，如计分和游戏结束判断等
}
