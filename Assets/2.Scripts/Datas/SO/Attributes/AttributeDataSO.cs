using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AttributeData", fileName = "AttributeData")]
public class AttributeDataSO : ListDataSO<AttributeData>
{
    private Dictionary<Rank, AttributeData> attributeDataDict = new Dictionary<Rank, AttributeData>();

    public AttributeData GetAttributeData(Rank rank)
    {
        if (attributeDataDict.Count == 0)
        {
            InitDict();
        }

        if (attributeDataDict.ContainsKey(rank))
        {
            return attributeDataDict[rank];
        }

        return default;
    }

    public override void InitDict()
    {
        attributeDataDict.Clear();

        foreach (var data in datas)
        {
            if (!attributeDataDict.ContainsKey(data.rank))
            {
                attributeDataDict.Add(data.rank, data);
            }
        }
        
    }
}
