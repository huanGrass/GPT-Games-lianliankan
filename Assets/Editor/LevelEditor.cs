using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDataAsset))]
public class LevelEditor : Editor
{
    private bool[,] grid;
    private float cellSize = 20f; // 每个格子的大小
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

        // 当numRows或numCols改变时重新初始化网格
        if (grid == null || grid.GetLength(0) != levelDataAsset.levelData.rows || grid.GetLength(1) != levelDataAsset.levelData.cols)
        {
            InitializeGrid(levelDataAsset);
        }

        // 绘制网格
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
        // 添加其他按钮和保存逻辑
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
                lastModifiedCell = new Vector2Int(-1, -1); // 重置最后修改的单元格
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
                grid[cellY, cellX] = !grid[cellY, cellX]; // 切换格子状态
                lastModifiedCell = currentCell;
                GUI.changed = true; // 标记GUI已更改
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
            Debug.LogError("无法保存网格：非空格子数量必须是偶数且大于1。");
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
