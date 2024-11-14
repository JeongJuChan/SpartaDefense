using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserLevelDataSO))]
public class UserLevelDataSOEditor : DataSOEditor<UserLevelDataSO>
{
    public static void LoadCSVToSO(UserLevelDataSO userLevelDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        userLevelDataSO.SetData(new UserLevelData(int.Parse(elements[0]), int.Parse(elements[1]), int.Parse(elements[2]), int.Parse(elements[3])));
        userLevelDataSO.InitData();
        EditorUtility.SetDirty(userLevelDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');


        dataSO.SetData(new UserLevelData(int.Parse(elements[0]), int.Parse(elements[1]), int.Parse(elements[2]), int.Parse(elements[3])));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
