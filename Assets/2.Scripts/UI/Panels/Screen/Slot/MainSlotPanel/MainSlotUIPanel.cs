using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSlotUIPanel : UI_Base, UIInitNeeded
{
    [SerializeField] private Transform heroSlotsparent;
    [SerializeField] private Transform castleSlotParent;

    private MainSlotElement[] mainSlotElements;
    private MainSlotElement[] castleSlotElements;

    [SerializeField] private float slotScaleMod = 2f;

    [SerializeField] private SlotDetailPanel slotDetailPanel; 

    private Sprite defaultRankSprite;

    public void Init()
    {
        mainSlotElements = heroSlotsparent.GetComponentsInChildren<MainSlotElement>();

        defaultRankSprite = mainSlotElements[0].GetRankDefaultSprite();

        slotDetailPanel.OnGetSlotImageSize += GetSlotElementSize;

        for (int i = 0; i < mainSlotElements.Length; i++)
        {
            mainSlotElements[i].Init(i);
            mainSlotElements[i].TrySetDefaultImage(ResourceManager.instance.forgeEquipmentResourceDataHandler.GetEquipmentSlotIconSprite(i));
            mainSlotElements[i].OnOpenSlotDetail += slotDetailPanel.OpenSlotDetailPanel;
            /*if (i > 0)
            {
                mainSlotElements[i].InitUnlockData(DEFAULT_COLLEAGUE_SLOT_FEATURE_ID);
            }*/
        }

        castleSlotElements = castleSlotParent.GetComponentsInChildren<MainSlotElement>();
    }

    public void UpdateMainSlotUI(EquipmentType equipmentType, Sprite rankSprite, Sprite slotSprite, int level)
    {
        mainSlotElements[(int)equipmentType - 1].UpdateSlotUIElement(equipmentType, rankSprite, slotSprite, level);
    }

    private float GetSlotElementSize()
    {
        return slotScaleMod;
    }

    public Sprite GetSlotDefaultRankSprite()
    {
        return defaultRankSprite;
    }

}
