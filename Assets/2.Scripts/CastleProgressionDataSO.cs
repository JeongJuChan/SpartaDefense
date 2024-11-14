using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/CastleProgressionData", fileName = "CastleProgressionData")]
public class CastleProgressionDataSO : ListDataSO<CastleProgressionData>
{
    private Dictionary<int, CastleProgressionData> castleProgressionDataDict = new Dictionary<int, CastleProgressionData>();

    public List<CastleProgressionData> GetCastleProgressionDatas()
    {
        return datas;
    }


    public CastleProgressionData GetCastleProgressionData(int level)
    {
        if (castleProgressionDataDict.Count == 0)
        {
            InitDict();
        }

        if (castleProgressionDataDict.ContainsKey(level))
        {
            return castleProgressionDataDict[level];
        }

        return default;
    }

    public int GetCastleMaxLevel()
    {
        if (castleProgressionDataDict.Count == 0)
        {
            InitDict();
        }

        return castleProgressionDataDict.Count;
    }

    public Sprite GetCastleSprite(int level)
    {
        if (castleProgressionDataDict.Count == 0)
        {
            InitDict();
        }

        if (castleProgressionDataDict.ContainsKey(level))
        {
            return castleProgressionDataDict[level].CastleSprite;
        }

        return default;
    }

    public override void InitDict()
    {
        castleProgressionDataDict.Clear();

        foreach (CastleProgressionData data in datas)
        {
            if (!castleProgressionDataDict.ContainsKey(data.Level))
            {
                castleProgressionDataDict.Add(data.Level, data);
            }
        }
    }
}
