using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageSlotComparedUIPanel : MonoBehaviour, ISlotComparisonInfo
{
    [SerializeField] private SlotComparisonInfoData comparisonInfoData;

    public SlotComparisonInfoData GetSlotUpdateInfo()
    {
        return comparisonInfoData;
    }
}
