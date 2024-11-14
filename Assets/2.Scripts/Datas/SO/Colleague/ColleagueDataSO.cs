using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Colleague/ColleagueDataSO", fileName = "ColleagueData")]
public class ColleagueDataSO : ListDataSO<ColleagueData>
{
    private Dictionary<Rank, Dictionary<ColleagueType, ColleagueData>> slotDataDict = new Dictionary<Rank, Dictionary<ColleagueType, ColleagueData>>();
    private Dictionary<string, ColleagueData> settingSlotDataDict = new Dictionary<string, ColleagueData>();
    private Dictionary<int, ColleagueData> indexSlotDataDict = new Dictionary<int, ColleagueData>();
    private Dictionary<ColleagueType, int> indexByColleagueTypeDict = new Dictionary<ColleagueType, int>();

    [field: SerializeField] public int defaultPoolCount { get; private set; } = 10;

    private const string PROJECTILE = "Projectile";

    private HashSet<string> projectileNameSet = new HashSet<string>();

    /*public HeroProjectile[] UpdateSlotDatasIndex(HeroProjectile[] heroProjectiles)
    {
        if (settingSlotDataDict.Count == 0)
        {
            InitSettingSlotDict();
        }

        foreach (var heroProjectile in heroProjectiles)
        {
            foreach (var slotData in settingSlotDataDict.Values)
            {
                if (projectileNameSet.Contains(heroProjectile.name))
                {
                    break;
                }

                string heroDefaultName = heroProjectile.name.Substring(0, heroProjectile.name.Length - PROJECTILE.Length);

                if (slotData.colleagueName.Contains(heroDefaultName))
                {
                    int i = slotData.index / Consts.PERCENT_DIVIDE_VALUE;
                    int result = i * Consts.PERCENT_DIVIDE_VALUE;
                    heroProjectile.SetIndex(result);
                    heroProjectile.SetSlotInfo(slotData.colleagueInfo);
                    slotDataDict[slotData.colleagueInfo.rank][slotData.colleagueInfo.colleagueType] = slotData;
                    projectileNameSet.Add(heroProjectile.name);
                    break;
                }
                *//*if (slotData.colleagueInfo.colleagueType != ColleagueType.Rtan)
                {
                }*//*
                else if (slotData.colleagueName == heroProjectile.name)
                {
                    heroProjectile.SetIndex(slotData.index);
                    heroProjectile.SetSlotInfo(slotData.colleagueInfo);
                    slotDataDict[slotData.colleagueInfo.rank][slotData.colleagueInfo.colleagueType] = slotData;
                    break;
                }
            }
        }

        return heroProjectiles;
    }*/

    public List<ColleagueData> GetColleagueDataAll()
    {
        return datas;
    }

    public List<ColleagueData> GetColleagueDatas(Rank rank)
    {
        if (slotDataDict.Count == 0)
        {
            InitDict();
        }

        List<ColleagueData> colleagueDatas = new List<ColleagueData>(datas);

        colleagueDatas.RemoveAll(x => x.colleagueInfo.rank != rank);

        return colleagueDatas;
    }

    public ColleagueData GetColleagueData(int index)
    {
        if (slotDataDict.Count == 0)
        {
            InitDict();
        }

        if (indexSlotDataDict.ContainsKey(index))
        {
            return indexSlotDataDict[index];
        }

        return default;
    }

    public int GetIndexByColleagueType(ColleagueType colleagueType)
    {
        if (indexByColleagueTypeDict.Count == 0)
        {
            InitDict();
        }

        if (indexByColleagueTypeDict.ContainsKey(colleagueType))
        {
            return indexByColleagueTypeDict[colleagueType];
        }

        return default;
    }

    public ColleagueInfo GetColleagueInfoByColleagueType(ColleagueType colleagueType)
    {
        int index = GetIndexByColleagueType(colleagueType);
        return GetColleagueData(index).colleagueInfo;
    }

    public ColleagueData GetSlotData(ColleagueInfo slotInfo)
    {
        if (slotDataDict.Count == 0)
        {
            InitDict();
        }

        if (slotDataDict.ContainsKey(slotInfo.rank))
        {
            if (slotDataDict[slotInfo.rank].ContainsKey(slotInfo.colleagueType))
            {
                return slotDataDict[slotInfo.rank][slotInfo.colleagueType];
            }
        }

        return default;
    }

    public override void InitDict()
    {
        slotDataDict.Clear();
        indexSlotDataDict.Clear();

        foreach (var data in datas)
        {
            ColleagueType colleagueType = data.colleagueInfo.colleagueType;

            if (!slotDataDict.ContainsKey(data.colleagueInfo.rank))
            {
                slotDataDict.Add(data.colleagueInfo.rank, new Dictionary<ColleagueType, ColleagueData>());
            }

            slotDataDict[data.colleagueInfo.rank].Add(colleagueType, data);

            if (!indexSlotDataDict.ContainsKey(data.index))
            {
                indexSlotDataDict.Add(data.index, data);
            }

            if (!indexByColleagueTypeDict.ContainsKey(colleagueType))
            {
                indexByColleagueTypeDict.Add(colleagueType, data.index);
            }
        }
    }

    private void InitSettingSlotDict()
    {
        foreach (var data in datas)
        {
            settingSlotDataDict.Add(data.colleagueName, data);
        }
    }
}
