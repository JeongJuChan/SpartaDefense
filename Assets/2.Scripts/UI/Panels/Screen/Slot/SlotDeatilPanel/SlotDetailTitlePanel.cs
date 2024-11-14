using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotDetailTitlePanel : MonoBehaviour
{
    [SerializeField] private SlotDetailPanel slotDetailPanel;
    [SerializeField] private Image slotRankImage;
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI slotTypeText;

    private void Awake()
    {
        slotDetailPanel.OnUpdateSlotDetailTitlePanel += UpdateSlotDetailTitlePanel;
    }

    private void UpdateSlotDetailTitlePanel(Sprite rankSprite, Sprite mainSprite, int level, Color color, string rankText, 
        string nameText, EquipmentType equipmentType, string slotTypeText, float elementSize)
    {
        slotImage.rectTransform.localScale = new Vector3(elementSize, elementSize, 1f);

        slotRankImage.sprite = rankSprite;
        slotImage.sprite = mainSprite;
        levelText.text = $"Lv.{level}";
        this.rankText.text = $"[{rankText}]";
        this.rankText.color = color;
        //this.nameText.text = slotType == ColleagueType.Rtan ? nameText : nameText.Split($"{slotTypeText}-")[1];
        this.nameText.color = color;
        this.slotTypeText.text = slotTypeText;
    }
}
