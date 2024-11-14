using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueIconData
{
    public ColleagueType colleagueType;
    public Sprite colleagueSprite;

    public ColleagueIconData(ColleagueType colleagueType, Sprite colleagueSprite)
    {
        this.colleagueType = colleagueType;
        this.colleagueSprite = colleagueSprite;
    }
}
