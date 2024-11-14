using System;
using UnityEngine;

[Serializable]
public struct CastleProgressionData
{
    public int Level;
    public string Name;
    public int RequiredCharacterLevel;
    public int ForgeSlotOpen;
    public int stageClear;
    public int BaseHP;
    public int BaseAttack;
    public int BaseDefense;
    public Sprite CastleSprite;

    public CastleProgressionData(int level, string name, int requiredCharacterLevel, int forgeSlotOpen, int stageClear, int baseHP, int baseAttack, int baseDefense, Sprite castleSprite)
    {
        Level = level;
        Name = name;
        RequiredCharacterLevel = requiredCharacterLevel;
        ForgeSlotOpen = forgeSlotOpen;
        this.stageClear = stageClear;
        BaseHP = baseHP;
        BaseAttack = baseAttack;
        BaseDefense = baseDefense;
        CastleSprite = castleSprite;
    }
}
