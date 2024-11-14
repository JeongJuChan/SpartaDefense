using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForgeEquipmentData", menuName = "SO/Equipment/ForgeEquipmentData")]
public class ForgeEquipmentDataSO : ListDataSO<ForgeEquipmentData>
{
    private Dictionary<EquipmentType, Dictionary<Rank, ForgeEquipmentData>> forgeEquipmentDataDict = new Dictionary<EquipmentType, Dictionary<Rank, ForgeEquipmentData>>();

    public ForgeEquipmentData GetForgeEquipmentData(ForgeEquipmentInfo forgeEquipmentInfo)
    {
        if (forgeEquipmentDataDict.Count == 0)
        {
            InitDict();
        }

        return forgeEquipmentDataDict[forgeEquipmentInfo.equipmentType][forgeEquipmentInfo.rank];
    }

    public override void InitDict()
    {
        foreach (ForgeEquipmentData data in datas)
        {
            ForgeEquipmentInfo forgeEquipmentInfo = data.forgeEquipmentInfo;
            EquipmentType equipmentType = forgeEquipmentInfo.equipmentType;
            Rank rank = forgeEquipmentInfo.rank;
            if (!forgeEquipmentDataDict.ContainsKey(equipmentType))
            {
                forgeEquipmentDataDict.Add(equipmentType, new Dictionary<Rank, ForgeEquipmentData>());
            }

            if (!forgeEquipmentDataDict[equipmentType].ContainsKey(rank))
            {
                forgeEquipmentDataDict[equipmentType].Add(rank, data);
            }
        }
    }
}
