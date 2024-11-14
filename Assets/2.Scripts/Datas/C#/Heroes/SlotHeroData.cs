using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotHeroData
{
    [field: SerializeField] public ColleagueInfo slotInfo { get; private set; }
    [field: SerializeField] public Sprite defaultSprite { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController runtimeAnimatorController { get; private set; }

    public SlotHeroData(ColleagueInfo slotInfo, Sprite defaultSprite, RuntimeAnimatorController runtimeAnimatorController)
    {
        this.slotInfo = slotInfo;
        this.defaultSprite = defaultSprite;
        this.runtimeAnimatorController = runtimeAnimatorController;
    }
}
