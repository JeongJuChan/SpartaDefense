using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotStatChangedUIPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private HpSlotComparedUIPanel hpSlotChangedUIPanel;
    [SerializeField] private DamageSlotComparedUIPanel damageSlotChangedUIPanel;
    [SerializeField] private DefenseSlotComparedUIPanel defenseStatChangedUIPanel;

    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    [SerializeField] private NewSlotUIPanel newSlotUIPanel;

    private const float MULTIPLIED_SIZE = 0.92f;

    private float unitDistance;

    private const float ONE_MULTIPLIED_DISTANCE = 0.55f;
    private const float SEVEN_MULTIPLIED_DISTANCE = 8f / 9f;
    private Dictionary<StatType, ISlotComparisonInfo> changedStatStateDict;
    public void Init()
    {        
        newSlotUIPanel.OnUpdateSummonUI += ChangeStatState;
        
        changedStatStateDict = new Dictionary<StatType, ISlotComparisonInfo>()
        {
            { StatType.HP, hpSlotChangedUIPanel },
            { StatType.Damage, damageSlotChangedUIPanel },
            { StatType.Defense, defenseStatChangedUIPanel },
        };

        unitDistance = changedStatStateDict[StatType.HP].GetSlotUpdateInfo().comparisonText.fontSize * MULTIPLIED_SIZE;
    }

    private void ChangeStatState(StatType statType, BigInteger origin, BigInteger after)
    {
        SlotComparisonInfoData slotComparisonInfoData = changedStatStateDict[statType].GetSlotUpdateInfo();

        UpdateImage(slotComparisonInfoData.upDownImage, slotComparisonInfoData.comparisonText.text, origin, after);
    }

    private void UpdateImage(Image upDownImage, string text, BigInteger origin, BigInteger after)
    {
        int originLength = text.Length;
        int oneLength = originLength - text.Trim('1').Length;
        int sevenLength = originLength - text.Trim('7').Length;
        int exceptOneOrSevenLegth = originLength - oneLength - sevenLength;

        float posX = unitDistance * exceptOneOrSevenLegth + unitDistance * ONE_MULTIPLIED_DISTANCE * oneLength +
            unitDistance * SEVEN_MULTIPLIED_DISTANCE * sevenLength;
        upDownImage.rectTransform.anchoredPosition = new Vector2(posX, upDownImage.rectTransform.anchoredPosition.y);

        if (origin == null)
        {
            upDownImage.gameObject.SetActive(true);
            upDownImage.sprite = upSprite;
            upDownImage.color = Color.green;
            return;
        }


        if (origin == after)
        {
            upDownImage.gameObject.SetActive(false);
        }
        else
        {
            upDownImage.gameObject.SetActive(true);

            if (origin > after)
            {
                upDownImage.sprite = downSprite;
                upDownImage.color = Color.red;
            }
            else
            {
                upDownImage.sprite = upSprite;
                upDownImage.color = Color.green;
            }
        }
    }
}
