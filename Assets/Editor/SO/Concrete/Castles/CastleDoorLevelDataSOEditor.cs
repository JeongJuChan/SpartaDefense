using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CastleDoorLevelDataSO))]
public class CastleDoorLevelDataSOEditor : DataSOEditor<CastleDoorLevelDataSO>
{
    public static void LoadCSVToSO(CastleDoorLevelDataSO castleDoorLevelDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        castleDoorLevelDataSO.SetData(new CastleDoorLevelData(int.Parse(elements[2]), int.Parse(elements[3]), int.Parse(elements[4].Trim('\r'))));
        castleDoorLevelDataSO.InitData();
        EditorUtility.SetDirty(castleDoorLevelDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        dataSO.SetData(new CastleDoorLevelData(int.Parse(elements[2]), int.Parse(elements[3]), int.Parse(elements[4].Trim('\r'))));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
