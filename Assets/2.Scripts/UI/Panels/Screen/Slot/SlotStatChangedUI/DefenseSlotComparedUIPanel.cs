using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseSlotComparedUIPanel : MonoBehaviour, ISlotComparisonInfo
{
    [SerializeField] private SlotComparisonInfoData comparisonInfoData;

    public SlotComparisonInfoData GetSlotUpdateInfo()
    {
        return comparisonInfoData;
    }
}
