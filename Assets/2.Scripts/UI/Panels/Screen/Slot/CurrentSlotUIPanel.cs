using System;
using UnityEngine;
using UnityEngine.UI;

public class CurrentSlotUIPanel : SlotUIPanel, IUIElement
{
    [SerializeField] private Image comparisonPanelImage;

    public event Func<float> OnGetSlotElementSize;

    public event Func<Sprite> OnGetSlotDefaultRankSprite;

    public event Action<bool> OnActiveSellButton;

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        OnActiveSellButton?.Invoke(isActive);
        comparisonPanelImage.enabled = isActive;
    }

    public void UpdateCurrentStatsText(SlotUIData slotUIData)
    {
        if (slotUIData.rankSprite == null)
        {
            slotUIData.rankSprite = OnGetSlotDefaultRankSprite.Invoke();
        }

        mainSlotElement.UpdateSlotUIElement(slotUIData.equipmentType, slotUIData.rankSprite, slotUIData.mainSprite,
            slotUIData.slotStatData.level);

        titleText.text = $"[{slotUIData.rankKR}]{slotUIData.nickName}";
        titleText.color = slotUIData.rankColor;
        if (slotUIData.equipmentName == null)
        {
            slotUIData.equipmentName = "";
        }
        typeText.text = slotUIData.equipmentName.ToString();

        if (slotUIData.slotStatData.equipmentStatData.health == null)
        {
            slotUIData.slotStatData.equipmentStatData.health = 0;
            slotUIData.slotStatData.equipmentStatData.mainDamage = 0;
            slotUIData.slotStatData.equipmentStatData.defense = 0;
        }

        hpText.text = slotUIData.slotStatData.equipmentStatData.health.ToString();

        damageText.text = slotUIData.slotStatData.equipmentStatData.mainDamage.ToString();

        defenseText.text = slotUIData.slotStatData.equipmentStatData.defense.ToString();

        OnUpdateSlotUIAttributes?.Invoke(slotUIData.attributeStatDict);
    }
}
