using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Equipment : UI_BottomElement
{
    [SerializeField] private Button[] equipmentTabs;
    [SerializeField] private GameObject[] tabIcons;
    [SerializeField] private Sprite tabNormalSprite;
    [SerializeField] private Sprite tabSelectedSprite;

    [SerializeField] private List<EquipmentSlot> slots;

    [SerializeField] private EquipmentUIHandler equipmentUIHandler;

    [Header("Colors")]
    [SerializeField] private Color tabNormalColor;
    [SerializeField] private Color tabSelectedColor;
    [SerializeField] private Color tabIconNormalColor;
    [SerializeField] private Color tabIconSelectedColor;

    [SerializeField] private GuideController guide;

    private EquipmentSlot selectedSlot;
    private bool initialized = false;

    public override void OpenUI()
    {
        base.OpenUI();

        Hes_Init();
    }

    public override void Initialize()
    {
        base.Initialize();
        NotificationManager.instance.SetNotification(RedDotIDType.ShowEquipmentButton, equipmentUIHandler.GetAvailables());
        InitSubUIs();
        // EquipmentManager.instance.InitializeChecks();

    }

    private void OnDisable()
    {
        NotificationManager.instance.SetNotification(RedDotIDType.ShowEquipmentButton, equipmentUIHandler.GetAvailables());
    }

    public void Hes_Init()
    {
        if (initialized) return;
        InitSubUIs();
        AddEventCallbacks();
        InitSlots();
        guide.Initialize();
        EquipmentManager.instance.InitializeChecks();
        initialized = true;
    }

    private void AddEventCallbacks()
    {

        for (int i = 0; i < equipmentTabs.Length; i++)
        {
            EquipmentType type = (EquipmentType)i;
            equipmentTabs[i].onClick.AddListener(() => SetSlotByType(type));
        }

        for (int i = 0; i < slots.Count; i++)
        {
            int idx = i;
            slots[idx].GetComponent<Button>().onClick.AddListener(() => SelectEquipment(idx));
        }
    }

    private void InitSlots()
    {
        SetSlotByType(EquipmentType.Bow);
    }

    private void InitSubUIs()
    {
        equipmentUIHandler.Initialize();
    }

    private void SetSlotByType(EquipmentType type)
    {
        List<EquipmentData> equipments = EquipmentManager.instance.GetDatasByType(type);

        for (int i = 0; i < equipments.Count; i++)
        {
            int index = i;
            equipments[index].RemoveAllCallbacks();
            slots[index].SetEquipmentInfo(equipments[index]);
            slots[index].OnSlotSelected += equipmentUIHandler.UpdateSelectedView;
        }

        int equippedIndex = equipments.IndexOf(EquipmentManager.instance.GetEquippedEquipment(type));
        equippedIndex = Mathf.Max(0, equippedIndex);

        SelectEquipment(equippedIndex);
        PositionScrollView(equippedIndex);

        UpdateTabUI(type);
    }

    private void UpdateTabUI(EquipmentType type)
    {
        foreach (Button tab in equipmentTabs)
        {
            tab.TryGetComponent(out Image image);
            image.sprite = tabNormalSprite;
            image.color = tabIconNormalColor;
        }

        for (int i = 0; i < equipmentTabs.Length; i++)
        {
            equipmentTabs[i].TryGetComponent(out Image image);
            image.sprite = tabNormalSprite;
            image.color = tabNormalColor;
            tabIcons[i].GetComponentInChildren<Image>().color = tabIconNormalColor;
        }

        equipmentTabs[(int)type].TryGetComponent(out Image selectedImage);
        selectedImage.sprite = tabSelectedSprite;
        selectedImage.color = tabSelectedColor;
        tabIcons[(int)type].GetComponentInChildren<Image>().color = tabIconSelectedColor;
    }

    private void SelectEquipment(int idx)
    {
        if (selectedSlot) selectedSlot.SetAsSelected(false);

        selectedSlot = slots[idx];

        selectedSlot.SetAsSelected(true);
    }

    private void PositionScrollView(int idx)
    {

    }

    private void RemoveCallbacks()
    {
        foreach (EquipmentSlot slot in slots)
        {
            slot.RemoveCallbacks();
        }
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
    }
}
