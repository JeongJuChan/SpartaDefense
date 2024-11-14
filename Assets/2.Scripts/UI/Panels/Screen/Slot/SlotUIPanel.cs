using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class SlotUIPanel : UI_Base
{
    [SerializeField] protected MainSlotElement mainSlotElement;

    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected TextMeshProUGUI typeText;
    [SerializeField] protected TextMeshProUGUI hpText;
    [SerializeField] protected TextMeshProUGUI damageText;
    [SerializeField] protected TextMeshProUGUI defenseText;

    public Action<Dictionary<int, float>> OnUpdateSlotUIAttributes;
}
