using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName; // �ؿ�����
    public int rows;
    public int cols;
    public int[] shapeTemplate; // ʹ��һά����

    // �Ӷ�ά���鵽һά�����ת��
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

    // ��һά���鵽��ά�����ת��
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
