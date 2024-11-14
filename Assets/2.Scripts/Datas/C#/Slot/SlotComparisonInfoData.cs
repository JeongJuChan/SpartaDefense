using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public struct SlotComparisonInfoData
{
    public TextMeshProUGUI comparisonText;
    public Image upDownImage;

    public SlotComparisonInfoData(TextMeshProUGUI comparisonText, Image upDownImage)
    {
        this.comparisonText = comparisonText;
        this.upDownImage = upDownImage;
    }
}
