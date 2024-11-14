using Keiwando.BigInteger;
using System;

[Serializable]
public struct SlotEquipmentStatData
{
    public BigInteger mainDamage;
    public BigInteger health;
    public BigInteger defense;
    public SlotEquipmentStatDataSave saveDatas;

    public SlotEquipmentStatData(BigInteger mainDamage, BigInteger health, BigInteger defense, SlotEquipmentStatDataSave saveDatas)
    {
        this.mainDamage = mainDamage;
        this.health = health;
        this.defense = defense;
        this.saveDatas = saveDatas;
    }

    public void SaveDatas(EquipmentType equipmentType, bool isCurrentSlot)
    {
        saveDatas = new SlotEquipmentStatDataSave(mainDamage, health, defense);

        ES3.Save<SlotEquipmentStatDataSave>($"SlotEquipmentStatDataSave_{equipmentType}{isCurrentSlot}", saveDatas, ES3.settings);
    }

    public void LoadDatas(EquipmentType equipmentType, bool isCurrentSlot)
    {
        if (ES3.KeyExists($"SlotEquipmentStatDataSave_{equipmentType}{isCurrentSlot}", ES3.settings))
        {
            saveDatas = ES3.Load<SlotEquipmentStatDataSave>($"SlotEquipmentStatDataSave_{equipmentType}{isCurrentSlot}", ES3.settings);
            mainDamage = new BigInteger(saveDatas.mainDamage);
            health = new BigInteger(saveDatas.health);
            defense = new BigInteger(saveDatas.defense);
        }
    }
}

[Serializable]
public struct SlotEquipmentStatDataSave
{
    public string mainDamage;
    public string health;
    public string defense;

    public SlotEquipmentStatDataSave(BigInteger mainDamage, BigInteger health, BigInteger defense)
    {
        this.mainDamage = mainDamage.ToString();
        this.health = health.ToString();
        this.defense = defense.ToString();
    }
}