using System;

[Serializable]
public struct ForgeEquipmentInfo
{
    public EquipmentType equipmentType;
    public Rank rank;

    public ForgeEquipmentInfo(EquipmentType equipmentType, Rank rank)
    {
        this.equipmentType = equipmentType;
        this.rank = rank;
    }
}
