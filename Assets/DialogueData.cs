using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData
{
    public DialogueType type;
    public int index;
    public string dialogueDescription;
    public int questArriveNum;

    public DialogueData(DialogueType type, int index, string dialogueDescription, int questArriveNum)
    {
        this.type = type;
        this.index = index;
        this.dialogueDescription = dialogueDescription;
        this.questArriveNum = questArriveNum;
    }
}
