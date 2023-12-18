using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDataAsset))]
public class LevelEditor : Editor
{
    private bool[,] grid;
    private float cellSize = 20f; // ÿ�����ӵĴ�С
    private bool isMouseDragging = false;
    private Vector2Int lastModifiedCell = new Vector2Int(-1, -1);

    private void OnEnable()
    {
        LevelDataAsset levelDataAsset = target as LevelDataAsset;
        if (levelDataAsset != null && levelDataAsset.levelData != null)
        {
            InitializeGrid(levelDataAsset);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelDataAsset levelDataAsset = (LevelDataAsset)target;

        // ��numRows��numCols�ı�ʱ���³�ʼ������
        if (grid == null || grid.GetLength(0) != levelDataAsset.levelData.rows || grid.GetLength(1) != levelDataAsset.levelData.cols)
        {
            InitializeGrid(levelDataAsset);
        }

        // ��������
        Rect gridRect = GUILayoutUtility.GetRect(levelDataAsset.levelData.cols * cellSize, levelDataAsset.levelData.rows * cellSize);
        HandleGridInput(gridRect, levelDataAsset);
        EditorGUI.DrawRect(gridRect, Color.gray);
        for (int i = 0; i < levelDataAsset.levelData.rows; i++)
        {
            for (int j = 0; j < levelDataAsset.levelData.cols; j++)
            {
                Rect cellRect = new Rect(gridRect.x + j * cellSize, gridRect.y + i * cellSize, cellSize, cellSize);
                EditorGUI.DrawRect(cellRect, grid[i, j] ? Color.black : Color.white);
            }
        }
        // ���������ť�ͱ����߼�
        DrawButtons(levelDataAsset);
    }

    private void InitializeGrid(LevelDataAsset levelDataAsset)
    {
        int numRows = levelDataAsset.levelData.rows;
        int numCols = levelDataAsset.levelData.cols;
        grid = new bool[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int index = i * numCols + j;
                if (index < levelDataAsset.levelData.shapeTemplate.Length)
                {
                    grid[i, j] = levelDataAsset.levelData.shapeTemplate[index] == 1;
                }
            }
        }
    }

    private void DrawButtons(LevelDataAsset levelDataAsset)
    {
        if (GUILayout.Button("Save Grid to Template"))
        {
            if (TrySaveGridToTemplate(levelDataAsset))
            {
                Debug.Log("Grid saved to template.");
            }
        }

        if (GUILayout.Button("Clear Grid"))
        {
            ClearGrid();
        }

        if (GUILayout.Button("Fill Grid"))
        {
            FillGrid();
        }
    }

    private void HandleGridInput(Rect gridRect, LevelDataAsset levelDataAsset)
    {
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        if (gridRect.Contains(mousePos))
        {
            if (e.type == EventType.MouseDown)
            {
                isMouseDragging = true;
                ToggleGridCell(mousePos, gridRect, levelDataAsset);
            }
            else if (e.type == EventType.MouseDrag && isMouseDragging)
            {
                ToggleGridCell(mousePos, gridRect, levelDataAsset);
            }
            else if (e.type == EventType.MouseUp)
            {
                isMouseDragging = false;
                lastModifiedCell = new Vector2Int(-1, -1); // ��������޸ĵĵ�Ԫ��
            }
        }
    }

    private void ToggleGridCell(Vector2 mousePos, Rect gridRect, LevelDataAsset levelDataAsset)
    {
        int numRows = levelDataAsset.levelData.rows;
        int numCols = levelDataAsset.levelData.cols;
        int cellX = (int)((mousePos.x - gridRect.x) / cellSize);
        int cellY = (int)((mousePos.y - gridRect.y) / cellSize);

        if (cellX >= 0 && cellX < numCols && cellY >= 0 && cellY < numRows)
        {
            Vector2Int currentCell = new Vector2Int(cellX, cellY);
            if (currentCell != lastModifiedCell)
            {
                grid[cellY, cellX] = !grid[cellY, cellX]; // �л�����״̬
                lastModifiedCell = currentCell;
                GUI.changed = true; // ���GUI�Ѹ���
            }
        }
    }

    private bool TrySaveGridToTemplate(LevelDataAsset levelDataAsset)
    {
        LevelData levelData = levelDataAsset.levelData;
        int filledCells = 0;
        foreach (bool cell in grid)
        {
            if (cell) filledCells++;
        }

        if (filledCells % 2 != 0 || filledCells <= 1)
        {
            Debug.LogError("�޷��������񣺷ǿո�������������ż���Ҵ���1��");
            return false;
        }

        levelData.shapeTemplate = new int[levelData.rows * levelData.cols];
        for (int i = 0; i < levelData.rows; i++)
        {
            for (int j = 0; j < levelData.cols; j++)
            {
                levelData.shapeTemplate[i * levelData.cols + j] = grid[i, j] ? 1 : 0;
            }
        }
        EditorUtility.SetDirty(levelDataAsset);
        return true;
    }

    private void ClearGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = false;
            }
        }
        GUI.changed = true;
    }

    private void FillGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = true;
            }
        }
        GUI.changed = true;
    }
}
