using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/CastleSprite", fileName = "CastleSprite")]
public class CastleSpriteSO : ScriptableObject
{
    [field: SerializeField] public CastleSpriteData[] castleSpriteData { get; private set; }
}
