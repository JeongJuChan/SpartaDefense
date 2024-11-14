using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotDetailPanel : UI_Popup, UIInitNeeded
{
    [SerializeField] private UnlockDataSO unlockDataSO;
    [SerializeField] private Image slotRankImage;
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI slotTypeText;

    [SerializeField] private Button disablePanel;

    public event Func<EquipmentType, SlotUIData> OnGetSlotStatUIData;
    public event Action<Sprite, Sprite, int, Color, string, string, EquipmentType, string, float> OnUpdateSlotDetailTitlePanel;
    public event Action<BigInteger, BigInteger, BigInteger> OnUpdateSlotDefaultStatUI;
    public event Action<Dictionary<int, float>> OnUpdateSlotDetailAttributePanel;

    public event Func<float> OnGetSlotImageSize;

    [SerializeField] private Image boundaryImage;
    private UI_Alert uI_Alert;


    public void Init()
    {
        ActiveSelf(false);
        disablePanel.onClick.AddListener(() => ActiveSelf(false));
        Initialize();

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();
    }

    public void OpenSlotDetailPanel(EquipmentType equipmentType, FeatureType featureType, int unlockCount)
    {
        if (equipmentType == EquipmentType.None)
        {
            /*FeatureID featureID = EnumUtility.GetEqualValue<FeatureID>($"ForgeSlot_{index + 1}");
            int stageIndex = unlockDataSO.GetUnlockData(featureID).count;
            if (stageIndex > currentIndex)
            {
                uI_Alert.AlertMessage("해당 슬롯은 잠겨있습니다. " + EnumToKR.TransformStageNumber(stageIndex) + " 스테이지에서 해제됩니다.");
            }*/
            return;
        }

        ActiveSelf(true);

        SlotUIData slotUIData = OnGetSlotStatUIData.Invoke(equipmentType);

        float slotImageSize;

        /*if (slotUIData.slotType == ColleagueType.Rtan || slotUIData.slotType == ColleagueType.None)
        {
            slotImageSize = 1;
        }
        else*/
        {
            slotImageSize = OnGetSlotImageSize.Invoke();
        }

        OnUpdateSlotDetailTitlePanel?.Invoke(slotUIData.rankSprite, slotUIData.mainSprite, slotUIData.slotStatData.level,
            slotUIData.rankColor, slotUIData.rankKR, slotUIData.nickName, slotUIData.equipmentType, slotUIData.equipmentName, 1);

        SlotStatData slotStatData = slotUIData.slotStatData;

        OnUpdateSlotDefaultStatUI?.Invoke(slotStatData.equipmentStatData.health, slotStatData.equipmentStatData.mainDamage,
            slotStatData.equipmentStatData.defense);

        if (slotUIData.attributeStatDict != null)
        {
            ActiveBoundaryPanel(true);
            OnUpdateSlotDetailAttributePanel?.Invoke(slotUIData.attributeStatDict);
        }
        else
        {
            ActiveBoundaryPanel(false);
        }
    }

    private void ActiveSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
        OnChangePopupState?.Invoke(isActive);
        disablePanel.gameObject.SetActive(isActive);
    }

    private void ActiveBoundaryPanel(bool isActive)
    {
        boundaryImage.gameObject.SetActive(isActive);
    }
}
