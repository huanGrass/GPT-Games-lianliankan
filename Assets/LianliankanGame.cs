using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class LianliankanGame : MonoBehaviour
{
    private GameObject[,] tiles;
    public AudioSource backgroundMusicSource; // �������ֵ�AudioSource
    public AudioSource clickSoundEffectSource; // �����Ч��AudioSource
    public AudioSource eliminationSoundEffectSource; // ������Ч��AudioSource

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
            // �������ȡ��ѡ����Ӿ�����
            HideSelectionEffect(); // ����֮ǰ��ѡ��Ч��
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
            HideSelectionEffect(); // ����֮ǰ��ѡ��Ч��
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
        currentSelectionEffect.transform.SetParent(tile.transform); // ������ false ��Ϊ�ڶ�������
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
            // ����Ƿ�����ͬ��ͼ��
            if (selectedTile1.GetComponent<Image>().sprite == selectedTile2.GetComponent<Image>().sprite)
            {
                // ������������Ƿ�������ӣ�����ȡ�սǵ�
                List<Vector2> cornerPoints = CanBeConnected(selectedTile1, selectedTile2);
                if (cornerPoints != null)
                {
                    // ����������ӣ����������߶�
                    DrawLinesForMatch(selectedTile1, selectedTile2, cornerPoints);

                    // �Ƴ�����
                    Destroy(selectedTile1);
                    Destroy(selectedTile2);
                    OnTilesRemoved();
                    eliminationSoundEffectSource.Play();
                }
            }

            // ����ѡ��
            selectedTile1 = null;
            selectedTile2 = null;
        }
    }

    public void OnReturnButtonClicked()
    {
        SceneManager.LoadScene("MainMenu"); // ���ؿ�ʼ����ĳ�����
    }

    List<Vector2> CanBeConnected(GameObject tile1, GameObject tile2)
    {
        // ��ȡ���������λ��
        Vector2 pos1 = GetPositionInGrid(tile1);
        Vector2 pos2 = GetPositionInGrid(tile2);

        // ����Ƿ���ͬһ�л�ͬһ��
        if (pos1.x == pos2.x || pos1.y == pos2.y)
        {
            if (IsPathClear(pos1, pos2))
            {
                return new List<Vector2>(); // ֱ�����ӣ�û�йս�
            }
        }

        // ���ͨ��һ��ת������
        List<Vector2> oneCornerConnection = IsOneCornerConnection(pos1, pos2);
        if (oneCornerConnection != null)
        {
            return oneCornerConnection;
        }

        // ���ͨ������ת������
        List<Vector2> twoCornerConnection = IsTwoCornerConnection(pos1, pos2);
        return twoCornerConnection; // ������ null�����û������ת������
    }


    bool IsPathClear(Vector2 pos1, Vector2 pos2)
    {
        if (pos1.x == pos2.x)
        {
            // ͬһ��
            int startY = (int)Mathf.Min(pos1.y, pos2.y);
            int endY = (int)Mathf.Max(pos1.y, pos2.y);
            for (int y = startY + 1; y < endY; y++)
            {
                if (tiles[(int)pos1.x, y] != null)
                {
                    // ·�����赲
                    return false;
                }
            }
        }
        else if (pos1.y == pos2.y)
        {
            // ͬһ��
            int startX = (int)Mathf.Min(pos1.x, pos2.x);
            int endX = (int)Mathf.Max(pos1.x, pos2.x);
            for (int x = startX + 1; x < endX; x++)
            {
                if (tiles[x, (int)pos1.y] != null)
                {
                    // ·�����赲
                    return false;
                }
            }
        }
        else
        {
            // ����ͬһ�л�ͬһ��
            return false;
        }

        // ·������
        return true;
    }


    List<Vector2> IsOneCornerConnection(Vector2 pos1, Vector2 pos2)
    {
        // ����һ�����ܵ�ת�ǵ�
        Vector2 corner1 = new Vector2(pos1.x, pos2.y);
        if (tiles[(int)corner1.x, (int)corner1.y] == null) // ȷ��ת�ǵ��ǿյ�
        {
            if (IsPathClear(pos1, corner1) && IsPathClear(corner1, pos2))
            {
                return new List<Vector2> { corner1 }; // ���ذ���һ���սǵ���б�
            }
        }

        // ���ڶ������ܵ�ת�ǵ�
        Vector2 corner2 = new Vector2(pos2.x, pos1.y);
        if (tiles[(int)corner2.x, (int)corner2.y] == null) // ȷ��ת�ǵ��ǿյ�
        {
            if (IsPathClear(pos1, corner2) && IsPathClear(corner2, pos2))
            {
                return new List<Vector2> { corner2 }; // ���ذ���һ���սǵ���б�
            }
        }

        return null; // ���û�йս����ӣ�����null
    }

    List<Vector2> IsTwoCornerConnection(Vector2 pos1, Vector2 pos2)
    {
        // ������pos1��pos2ͬ�л�ͬ�еĿո���
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            if (tiles[x, (int)pos1.y] == null && IsPathClear(pos1, new Vector2(x, pos1.y)))
            {
                Vector2 corner1 = new Vector2(x, pos1.y);
                List<Vector2> corners = IsOneCornerConnection(corner1, pos2);
                if (corners != null)
                {
                    corners.Insert(0, corner1); // ����һ���սǵ���ӵ��б�ʼλ��
                    return corners;
                }
            }
            if (tiles[x, (int)pos2.y] == null && IsPathClear(pos2, new Vector2(x, pos2.y)))
            {
                Vector2 corner1 = new Vector2(x, pos2.y);
                List<Vector2> corners = IsOneCornerConnection(pos1, corner1);
                if (corners != null)
                {
                    corners.Add(corner1); // ����һ���սǵ���ӵ��б�ĩβ
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

        return null; // ���û�������ս����ӣ�����null
    }


    Vector2 GetPositionInGrid(GameObject tile)
    {
        string name = tile.name;
        // �������Ƹ�ʽΪ "Tile_x_y"������x��y������λ��
        string[] parts = name.Split('_');
        if (parts.Length == 3)
        {
            int x, y;
            if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
            {
                return new Vector2(x, y);
            }
        }

        Debug.LogError("�޷��ӷ������ƽ���λ��: " + name);
        return new Vector2(-1, -1);
    }




    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // �����������ѡ���߼�
        }
    }

    bool CheckMatch(GameObject tile1, GameObject tile2)
    {
        // ����Ƿ��������tile1��tile2
        return false;
    }

    void RemoveTiles(GameObject tile1, GameObject tile2)
    {
        // �Ƴ�ƥ��ķ���
    }
    public GameObject lineImagePrefab; // �ڱ༭���������������

    void DrawLinesForMatch(GameObject tile1, GameObject tile2, List<Vector2> cornerPoints)
    {
        Vector2 startPos = tile1.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = tile2.GetComponent<RectTransform>().anchoredPosition;

        // ת���սǵ�����
        List<Vector2> uiCornerPoints = cornerPoints.Select(p => GridIndexToUIPosition((int)p.x, (int)p.y)).ToList();

        // �������е���б���㡢�м�սǵ��UIλ�á��յ㣩
        List<Vector2> points = new List<Vector2> { startPos };
        points.AddRange(uiCornerPoints);
        points.Add(endPos);

        // Ϊÿ��·������һ����
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLineBetweenPoints(points[i], points[i + 1]);
        }
    }
    Vector2 GridIndexToUIPosition(int x, int y)
    {
        float tileWidth = 50f;
        float tileHeight = 50f;
        // ����tiles�ĵ�һ��������Ӧ����y���꣬�ڶ���������Ӧ����x���꣬������Ҫ����x��y��λ��
        return new Vector2(y * tileWidth, -x * tileHeight);
    }


    void DrawLineBetweenPoints(Vector2 start, Vector2 end)
    {
        GameObject lineObj = Instantiate(lineImagePrefab, transform);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        // �����߶ε�λ��
        lineRect.anchoredPosition = (start + end) / 2;

        // ȷ����ˮƽ���Ǵ�ֱ�߶Σ������ó���
        if (start.x == end.x) // ��ֱ�߶�
        {
            float length = Mathf.Abs(end.y - start.y);
            lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, length);
        }
        else // ˮƽ�߶�
        {
            float length = Mathf.Abs(end.x - start.x);
            lineRect.sizeDelta = new Vector2(length, lineRect.sizeDelta.y);
        }

        // ���ö�ʱ�������߶�
        Destroy(lineObj, 2f); // ����2�������
    }


    // ����������������Ʒֺ���Ϸ�����жϵ�
}
