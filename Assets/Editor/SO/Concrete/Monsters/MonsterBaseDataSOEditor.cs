using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterBaseStatDataSO))]
public class MonsterBaseDataSOEditor : DataSOEditor<MonsterBaseStatDataSO>
{
    public static void LoadCSVToSO(MonsterBaseStatDataSO monsterBaseStatDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        monsterBaseStatDataSO.SetData(new MonsterUpgradableCSVData(elements[2], elements[3], float.Parse(elements[4]), float.Parse(elements[5].Trim('\r'))));
        monsterBaseStatDataSO.InitData();
        EditorUtility.SetDirty(monsterBaseStatDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        dataSO.SetData(new MonsterUpgradableCSVData(elements[2], elements[3], float.Parse(elements[4]), float.Parse(elements[5].Trim('\r'))));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
