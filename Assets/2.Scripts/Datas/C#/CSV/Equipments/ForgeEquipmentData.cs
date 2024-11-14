using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ForgeEquipmentData
{
    public int index;
    public string equipmentName;
    public string equipmentNameKR;
    public ForgeEquipmentInfo forgeEquipmentInfo;

    public ForgeEquipmentData(int index, string equipmentName, string equipmentNameKR, ForgeEquipmentInfo forgeEquipmentInfo)
    {
        this.index = index;
        this.equipmentName = equipmentName;
        this.equipmentNameKR = equipmentNameKR;
        this.forgeEquipmentInfo = forgeEquipmentInfo;
    }
}
