using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotEvent
{
    public event Action<ColleagueType> OnOpenSlot;

    public void CallOpenSlot(ColleagueType slotType)
    {
        OnOpenSlot?.Invoke(slotType);
    }
}
