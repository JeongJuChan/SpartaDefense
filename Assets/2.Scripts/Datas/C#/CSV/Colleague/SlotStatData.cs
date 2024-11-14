using Keiwando.BigInteger;
using System;
using Unity.VisualScripting;

[Serializable]
public struct SlotStatData
{
    public Rank rank;
    public int level;
    public SlotEquipmentStatData equipmentStatData;
    public BigInteger gold;
    public BigInteger exp;
    public int increment;
    public int variationPercent;
    public int heroDamagePercent;
    public SlotStatDataSave saveDatas;

    public SlotStatData(Rank rank, int level, SlotEquipmentStatData equipmentStatData, BigInteger gold, BigInteger exp,
        int increment, int variationPercent, int heroDamagePercent, SlotStatDataSave saveDatas)
    {
        this.rank = rank;
        this.level = level;
        this.equipmentStatData = equipmentStatData;
        this.gold = gold;
        this.exp = exp;
        this.increment = increment;
        this.variationPercent = variationPercent;
        this.heroDamagePercent = heroDamagePercent;
        this.saveDatas = saveDatas;
    }

    public void SaveDatas(EquipmentType equipmentType, bool isCurrentSlot)
    {
        saveDatas = new SlotStatDataSave(exp, gold);

        ES3.Save<SlotStatDataSave>($"SlotStatDataSave_{equipmentType}{isCurrentSlot}", saveDatas, ES3.settings);

        equipmentStatData.SaveDatas(equipmentType, isCurrentSlot);
    }

    public void LoadDatas(EquipmentType equipmentType, bool isCurrentSlot)
    {
        if (ES3.KeyExists($"SlotStatDataSave_{equipmentType}{isCurrentSlot}", ES3.settings))
        {
            saveDatas = ES3.Load<SlotStatDataSave>($"SlotStatDataSave_{equipmentType}{isCurrentSlot}", ES3.settings);
            exp = new BigInteger(saveDatas.exp);
            gold = new BigInteger(saveDatas.gold);
        }

        equipmentStatData.LoadDatas(equipmentType, isCurrentSlot);
    }
}

[Serializable]
public struct SlotStatDataSave
{
    public string exp;
    public string gold;

    public SlotStatDataSave(BigInteger exp, BigInteger gold)
    {
        this.exp = exp.ToString();
        this.gold = gold.ToString();
    }
}