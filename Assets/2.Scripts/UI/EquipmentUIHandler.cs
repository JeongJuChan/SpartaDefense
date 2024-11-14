using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public struct ButtonInfo
{
    public Button button;
    public TextMeshProUGUI text;
    public GameObject redDot;
}

// public class EquipmentsData
// {
//     public string Rank { get; set; }
//     public Dictionary<EquipmentType, int> Owned { get; set; } = new Dictionary<EquipmentType, int>();
//     public Dictionary<EquipmentType, int> Equipped { get; set; } = new Dictionary<EquipmentType, int>();
// }

public class EquipmentUIHandler : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] EquipmentSlot preview;

    [Header("Descriptions")]
    [SerializeField] TextMeshProUGUI equippedEffectDesc;
    [SerializeField] TextMeshProUGUI ownerEffectDesc;

    [Header("Buttons")]
    [SerializeField] ButtonInfo levelUp;
    [SerializeField] ButtonInfo composite;
    [SerializeField] ButtonInfo equip;

    [SerializeField] ButtonInfo allComposite;
    [SerializeField] ButtonInfo recommanded;

    [Header("Colors")]
    [SerializeField] private Color optionLevleUpColor;
    [SerializeField] private Color equipColor;
    [SerializeField] private Color compositeAllColor;
    [SerializeField] private Color recommandedColor;

    [SerializeField] private Color buttonDisabledColor;

    [SerializeField] private Color textNormalColor;
    [SerializeField] private Color textDisabledColor;
    private UI_EquipmentLevelUp ui_equipmentLevelUp;



    private EquipmentData equipment;
    // private List<EquipmentsData> equipmentsData;

    private UI_Alert alert;

    private bool isInitialized;

    private bool isLevelUpAvailable;
    private bool isCompositeAvailable;
    private bool isRecommandAvailable;

    public void Initialize()
    {
        if (isInitialized) return;

        AddCallbacks();

        ui_equipmentLevelUp = UIManager.instance.GetUIElement<UI_EquipmentLevelUp>();
        alert = UIManager.instance.GetUIElement<UI_Alert>();

        ui_equipmentLevelUp.Initialize();


        isInitialized = true;
    }

    public bool GetAvailables()
    {
        return isCompositeAvailable || isRecommandAvailable;
    }
    private void AddCallbacks()
    {
        EquipmentManager.instance.OnRecommandedAvailable += UpdateRecommandBtn;
        EquipmentManager.instance.OnCompositeAllAvailable += UpdateAllCompositeBtn;

        levelUp.button.onClick.AddListener(LevelUpCallback);
        equip.button.onClick.AddListener(EquipCallback);
        composite.button.onClick.AddListener(CompositeCallback);
        recommanded.button.onClick.AddListener(RecommandedCallback);
        allComposite.button.onClick.AddListener(AllCompositeCallback);
    }

    public void UpdateSelectedView(EquipmentData equipment)
    {
        Initialize();

        if (this.equipment != null)
        {
            preview.RemoveCallbacks();

            this.equipment.OnCountChange -= ActivateButtons;
            this.equipment.OnLevelChange -= ActivateButtons;
            this.equipment.OnEquippedChange -= ActivateButtons;

            this.equipment.OnLevelChange -= UpdateEquippedEffectUI;
        }


        composite.button.image.color = (equipment.GetCount() >= 4) ? compositeAllColor : buttonDisabledColor;
        composite.text.color = (equipment.GetCount() >= 4) ? textNormalColor : textDisabledColor;

        this.equipment = equipment;

        preview.SetEquipmentInfo(equipment);

        equipment.OnCountChange += ActivateButtons;
        equipment.OnLevelChange += ActivateButtons;
        equipment.OnEquippedChange += ActivateButtons;

        equipment.OnLevelChange += UpdateEquippedEffectUI;

        ActivateButtons();
        SetEffectDescriptions();
    }

    private void ActivateButtons()
    {
        if (equipment.GetCount() > 0 || equipment.GetLevel() > 0)
        {
            composite.button.gameObject.SetActive(true);
            equip.button.gameObject.SetActive(EquipmentManager.instance.GetEquippedEquipment(equipment.EquipmentType) != equipment);
            levelUp.button.gameObject.SetActive(!equipment.IsAtMaxLevel());

        }
        else
        {
            composite.button.gameObject.SetActive(false);
            equip.button.gameObject.SetActive(false);
            levelUp.button.gameObject.SetActive(false);
        }

        UpdateLevelUpBtn(true);
    }

    private void SetEffectDescriptions()
    {
        UpdateEquippedEffectUI();
        UpdateOwnerEffectUI();
    }

    private void UpdateEquippedEffectUI()
    {
        equippedEffectDesc.text = $"{EnumToKRManager.instance.GetEnumToKR(equipment.effectType)} : {EquipmentManager.instance.GetEquipEffectValue(equipment)}";
    }

    private void UpdateOwnerEffectUI()
    {
        ownerEffectDesc.text = $"{EnumToKRManager.instance.GetEnumToKR(equipment.effectType)} : {EquipmentManager.instance.GetOwnerEffectValue(equipment)}";
    }

    private void UpdateLevelUpBtn(bool isAvailable)
    {
        isLevelUpAvailable = isAvailable;
        levelUp.button.image.color = (isLevelUpAvailable) ? optionLevleUpColor : buttonDisabledColor;
        levelUp.text.color = (isLevelUpAvailable) ? textNormalColor : textDisabledColor;
    }

    private void UpdateRecommandBtn(bool isAvailable)
    {
        isRecommandAvailable = isAvailable;
        recommanded.button.image.color = (isAvailable) ? recommandedColor : buttonDisabledColor;
        recommanded.text.color = (isAvailable) ? textNormalColor : textDisabledColor;

        NotificationManager.instance.SetNotification(RedDotIDType.EquipmentRecommandButton, isRecommandAvailable);

        if (gameObject.activeSelf)
            NotificationManager.instance.SetNotification(RedDotIDType.ShowEquipmentButton, GetAvailables());
    }

    private void UpdateAllCompositeBtn(bool isAvailable)
    {
        isCompositeAvailable = isAvailable;
        allComposite.button.image.color = (isAvailable) ? compositeAllColor : buttonDisabledColor;
        allComposite.text.color = (isAvailable) ? textNormalColor : textDisabledColor;

        NotificationManager.instance.SetNotification(RedDotIDType.EquipmentCompositeButton, isCompositeAvailable);

        if (gameObject.activeSelf)
            NotificationManager.instance.SetNotification(RedDotIDType.ShowEquipmentButton, GetAvailables());
    }

    private void LevelUpCallback()
    {
        if (!isLevelUpAvailable) return;
        if (equipment.IsAtMaxLevel()) return;
        else
        {
            //TODO: LevelUpPanel 띄워주기

            ui_equipmentLevelUp.SetEquipment(equipment);
            ui_equipmentLevelUp.OpenUI();

        }
    }

    private void EquipCallback()
    {
        EquipmentManager.instance.Equip(equipment);
    }

    private void RecommandedCallback()
    {
        if (!isRecommandAvailable)
        {
            alert.AlertMessage("추천할 장비가 없습니다.");
            // "추천할 장비가 없습니다." alert 띄우기
            return;
        }
        else EquipmentManager.instance.EquipRecommanded();
        NotificationManager.instance.SetNotification(RedDotIDType.EquipmentRecommandButton, false);

    }

    private void CompositeCallback()
    {
        if (!isCompositeAvailable)
        {
            alert.AlertMessage("합성할 장비가 없습니다.");
            return;
        }
        else EquipmentManager.instance.CompositeEquipment(equipment);
    }

    private void AllCompositeCallback()
    {
        if (!isCompositeAvailable)
        {
            alert.AlertMessage("합성할 장비가 없습니다.");
            return;
        }
        else EquipmentManager.instance.CompositeAllEquipments();
        NotificationManager.instance.SetNotification(RedDotIDType.EquipmentCompositeButton, false);
    }

    private void RemoveCallbacks()
    {
        preview.RemoveCallbacks();

        EquipmentManager.instance.OnRecommandedAvailable -= UpdateRecommandBtn;
        EquipmentManager.instance.OnCompositeAllAvailable -= UpdateAllCompositeBtn;

        equipment.OnCountChange -= ActivateButtons;
        equipment.OnLevelChange -= ActivateButtons;
        equipment.OnEquippedChange -= ActivateButtons;

        levelUp.button.onClick.RemoveAllListeners();
        equip.button.onClick.RemoveAllListeners();
        recommanded.button.onClick.RemoveAllListeners();
        allComposite.button.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
    }
}
