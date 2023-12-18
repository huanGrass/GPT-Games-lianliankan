using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName; // 关卡名称
    public int rows;
    public int cols;
    public int[] shapeTemplate; // 使用一维数组

    // 从二维数组到一维数组的转换
    public void SetShapeTemplateFrom2D(int[,] template)
    {
        rows = template.GetLength(0);
        cols = template.GetLength(1);
        shapeTemplate = new int[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                shapeTemplate[i * cols + j] = template[i, j];
            }
        }
    }

    // 从一维数组到二维数组的转换
    public int[,] GetShapeTemplateAs2D()
    {
        int[,] template = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                template[i, j] = shapeTemplate[i * cols + j];
            }
        }
        return template;
    }
}


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Create New Level Data", order = 1)]
public class LevelDataAsset : ScriptableObject
{
    public LevelData levelData;
}
