using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundSprite", menuName = "SO/Background/BackgroundSprite")]
public class BackgroundSpriteSO : ScriptableObject
{
    [SerializeField] private Sprite[] backgroundSprites;

    public Sprite GetBackgroundSprites(int mainStageNum)
    {
        return backgroundSprites[mainStageNum - 1];
    }
}
