using System;
using UnityEngine;

[Serializable]
public struct ColleagueUIData
{
    public int index;
    public int skillIndex;
    public ColleagueType colleagueType;
    public Rank rank;
    public string colleagueName;
    public string colleagueNameKR;
    public Sprite mainSprite;
    public Sprite backgroundSprite;
    public Sprite equipStateSprite;

    public ColleagueUIData(int index, int skillIndex, ColleagueType colleagueType, Rank rank, string colleagueName, 
        string colleagueNameKR, Sprite mainSprite, Sprite backgroundSprite, Sprite equipStateSprite)
    {
        this.index = index;
        this.skillIndex = skillIndex;
        this.colleagueType = colleagueType;
        this.rank = rank;
        this.colleagueName = colleagueName;
        this.colleagueNameKR = colleagueNameKR;
        this.mainSprite = mainSprite;
        this.backgroundSprite = backgroundSprite;
        this.equipStateSprite = equipStateSprite;
    }
}