using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeEquipmentResourceDataHandler
{
    private Dictionary<ForgeEquipmentInfo, Sprite> forgeEquipmentSpriteDict = new Dictionary<ForgeEquipmentInfo, Sprite>();
    private Dictionary<int, Sprite> equipmentSlotIconSpriteDict = new Dictionary<int, Sprite>();

    private const string DEFAULT_PATH = "Sprites/Equipments/";
    private const string ICON_PATH = "UI/MainSlotIcons/";

    public Sprite GetForgeEquipmentSprite(ForgeEquipmentInfo forgeEquipmentInfo)
    {
        if (forgeEquipmentSpriteDict.Count == 0)
        {
            InitDict();
        }

        return forgeEquipmentSpriteDict[forgeEquipmentInfo];
    }

    public Sprite GetEquipmentSlotIconSprite(int slotIndex)
    {
        if (equipmentSlotIconSpriteDict.Count == 0)
        {
            InitDict();
        }

        return equipmentSlotIconSpriteDict[slotIndex];
    }

    private void InitDict()
    {
        EquipmentType[] equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));
        Rank[] ranks = (Rank[])Enum.GetValues(typeof(Rank));

        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            EquipmentType equipmentType = equipmentTypes[i];
            for (int j = 1; j < ranks.Length; j++)
            {
                ForgeEquipmentInfo forgeEquipmentInfo = new ForgeEquipmentInfo(equipmentType, ranks[j]);
                Sprite sprite = Resources.Load<Sprite>($"{DEFAULT_PATH}{equipmentType}/{equipmentType}_{j - 1}");
                if (!forgeEquipmentSpriteDict.ContainsKey(forgeEquipmentInfo))
                {
                    forgeEquipmentSpriteDict.Add(forgeEquipmentInfo, sprite);
                }
            }

            Sprite equipmentIconSprite = Resources.Load<Sprite>($"{ICON_PATH}danjo_icon_{i - 1}");
            if (!equipmentSlotIconSpriteDict.ContainsKey(i - 1))
            {
                equipmentSlotIconSpriteDict.Add(i - 1, equipmentIconSprite);
            }
        }
    }
}
