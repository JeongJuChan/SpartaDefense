using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/DialogueData", fileName = "DialogueData")]
public class DialogueDataSO : ListDataSO<DialogueData>
{
    private Dictionary<DialogueType, DialogueData> dialogueDict = new Dictionary<DialogueType, DialogueData>();

    public Dictionary<DialogueType, DialogueData> GetDialogueDict()
    {
        if (dialogueDict.Count == 0)
        {
            SetDialogueDict();
        }
        return dialogueDict;
    }

    private void SetDialogueDict()
    {
        foreach (DialogueData data in datas)
        {
            if (!dialogueDict.ContainsKey(data.type))
            {
                dialogueDict.Add(data.type, data);
            }
        }
    }

    public override void InitDict()
    {
        dialogueDict.Clear();

        SetDialogueDict();
    }
}
