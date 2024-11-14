using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueDataSO))]
public class DialogueDataSOEditor : ListDataSOEditor<DialogueDataSO>
{
    public static void LoadCSVToSO(DialogueDataSO dialogueDataSO, TextAsset csv)
    {
        dialogueDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DialogueData> dialogueDatas = new List<DialogueData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            DialogueType dialogueType = EnumUtility.GetEqualValue<DialogueType>(elements[0]);
            int index = int.Parse(elements[1]);
            string dialogueDescription = elements[2];
            int questArriveNum = int.Parse(elements[3]);

            dialogueDatas.Add(new DialogueData(dialogueType, index, dialogueDescription, questArriveNum));
        }

        dialogueDataSO.AddDatas(dialogueDatas);
        dialogueDataSO.InitDict();
        EditorUtility.SetDirty(dialogueDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DialogueData> dialogueDatas = new List<DialogueData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            DialogueType dialogueType = EnumUtility.GetEqualValue<DialogueType>(elements[0]);
            int index = int.Parse(elements[1]);
            string dialogueDescription = elements[2];
            int questArriveNum = int.Parse(elements[3]);

            dialogueDatas.Add(new DialogueData(dialogueType, index, dialogueDescription, questArriveNum));
        }

        dataSO.AddDatas(dialogueDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}