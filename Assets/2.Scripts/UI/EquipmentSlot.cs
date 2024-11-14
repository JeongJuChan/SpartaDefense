using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    private bool isInitialized;

    [SerializeField] private Text rankText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text countText;
    [SerializeField] private Slider countBar;
    [SerializeField] private Image icon;
    [SerializeField] private Image iconShadow;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject selectedOutline;
    [SerializeField] GameObject equipMark;
    [SerializeField] private GameObject notPoccessed;

    public event Action<EquipmentData> OnSlotSelected;

    private EquipmentData equipment;

    public void Initialize()
    {
        if (isInitialized) return;

        equipMark.SetActive(false);

        isInitialized = true;
    }

    public void SetEquipmentInfo(EquipmentData equipment)
    {
        Initialize();

        selectedOutline.SetActive(false);
        this.equipment = equipment;

        icon.sprite = EquipmentManager.instance.GetIcon(equipment);
        iconShadow.sprite = EquipmentManager.instance.GetIcon(equipment);
        backgroundImage.color = ResourceManager.instance.rank.GetRankColor(equipment.rank);
        // rankText.color = ResourceManager.instance.rank.GetRankColor(equipment.rank);
        // rankText.text = $"{equipment.rank}";
        rankText.text = EnumToKRManager.instance.GetEnumToKR(equipment.rank);

        UpdateCountUI();
        UpdateLevelUI();
        UpdateEquipUI();

        equipment.OnCountChange += UpdateCountUI;
        equipment.OnLevelChange += UpdateLevelUI;
        equipment.OnEquippedChange += UpdateEquipUI;

        equipment.UpdateLevelEvent += UpdateLevelUI;
    }

    public void SetAsSelected(bool isSelected)
    {
        if (isSelected == selectedOutline.activeSelf) return;

        if (isSelected)
        {
            OnSlotSelected?.Invoke(equipment);
        }

        selectedOutline.SetActive(isSelected);
    }

    public void UpdateCountUI()
    {
        int count = equipment.GetCount();
        if (count > 0 || equipment.GetLevel() > 0)
        {
            countText.text = $"{count}/4";
            countBar.value = count / 4f;
            notPoccessed.SetActive(false);
        }
        else
        {
            countText.text = "0/0";
            countBar.value = 0;
            notPoccessed.SetActive(true);
        }
    }

    public void UpdateLevelUI()
    {
        int level = equipment.GetLevel();
        levelText.text = $"Lv.{level}";
    }

    public void UpdateEquipUI()
    {
        bool isEquipped = equipment.GetEquippedState();
        equipMark.SetActive(isEquipped);
    }

    public void RemoveCallbacks()
    {
        OnSlotSelected = null;

        if (equipment == null) return;
        equipment.OnCountChange -= UpdateCountUI;
        equipment.OnLevelChange -= UpdateLevelUI;
        equipment.OnEquippedChange -= UpdateEquipUI;
    }
}
