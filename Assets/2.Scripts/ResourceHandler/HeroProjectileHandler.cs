using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroProjectileHandler
{
    private Dictionary<int, GameObject> heroProjectileResourceDict = new Dictionary<int, GameObject>();
    private Dictionary<ColleagueType, Dictionary<Rank, GameObject>> colleagueProjectileResourceDict = new Dictionary<ColleagueType, Dictionary<Rank, GameObject>>();
    private const string HERO_PROJECTILE_PATH = "Projectiles/Heroes";

    public event Func<ColleagueType, Rank, ColleagueProjectileData> OnGetColleagueProjectileData;

    public HeroProjectileHandler(Func<ColleagueType, Rank, ColleagueProjectileData> getColleagueProjectileData)
    {
        OnGetColleagueProjectileData += getColleagueProjectileData;
    }

    public GameObject GetResource(ColleagueInfo colleagueInfo)
    {
        if (colleagueProjectileResourceDict.ContainsKey(colleagueInfo.colleagueType))
        {
            if (colleagueProjectileResourceDict[colleagueInfo.colleagueType].ContainsKey(colleagueInfo.rank))
            {
                return colleagueProjectileResourceDict[colleagueInfo.colleagueType][colleagueInfo.rank];
            }
        }

        return null;
    }

    public void LoadSlotTypeProjectile(ColleagueInfo colleagueInfo)
    {
        ColleagueProjectileData colleagueProjectileData = OnGetColleagueProjectileData.Invoke(colleagueInfo.colleagueType, colleagueInfo.rank);
        string middlePath = colleagueInfo.colleagueType.ToString().Split('_')[0];
        HeroProjectile projectile = Resources.Load<HeroProjectile>($"{HERO_PROJECTILE_PATH}/{middlePath}/{colleagueInfo.colleagueType}");

        if (!heroProjectileResourceDict.ContainsKey(colleagueProjectileData.index))
        {
            projectile.SetSlotInfo(colleagueInfo);
            projectile.SetIndex(colleagueProjectileData.index);
            heroProjectileResourceDict.Add(colleagueProjectileData.index, projectile.gameObject);
        }

        ColleagueType colleagueType = colleagueInfo.colleagueType;
        Rank rank = colleagueInfo.rank;

        if (!colleagueProjectileResourceDict.ContainsKey(colleagueInfo.colleagueType))
        {
            colleagueProjectileResourceDict.Add(colleagueInfo.colleagueType, new Dictionary<Rank, GameObject>());
        }

        if (!colleagueProjectileResourceDict[colleagueType].ContainsKey(rank))
        {
            colleagueProjectileResourceDict[colleagueType].Add(rank, projectile.gameObject);
        }

        /*foreach (var projectile in projectiles)
        {
            if (!heroProjectileResourceDict.ContainsKey(projectile.index))
            {
                heroProjectileResourceDict.Add(projectile.index, projectile.gameObject);
            }
        }*/
    }

    public void LoadSlotTypeProjectile(ColleagueInfo colleagueInfo, Rank projectileRank)
    {
        ColleagueProjectileData colleagueProjectileData = OnGetColleagueProjectileData.Invoke(colleagueInfo.colleagueType, colleagueInfo.rank);
        string middlePath = colleagueInfo.colleagueType.ToString().Split('_')[0];
        string colleagueName = colleagueInfo.colleagueType.ToString().Split('_')[0];
        HeroProjectile projectile = Resources.Load<HeroProjectile>($"{HERO_PROJECTILE_PATH}/{middlePath}/{colleagueName}_{projectileRank}");

        projectile.SetSlotInfo(new ColleagueInfo(projectileRank, colleagueInfo.colleagueType));
        projectile.SetIndex(colleagueProjectileData.index);
        if (!heroProjectileResourceDict.ContainsKey(colleagueProjectileData.index))
        {
            heroProjectileResourceDict.Add(colleagueProjectileData.index, projectile.gameObject);
        }
        else
        {
            heroProjectileResourceDict[colleagueProjectileData.index] = projectile.gameObject;
        }

        ColleagueType colleagueType = colleagueInfo.colleagueType;
        Rank rank = colleagueInfo.rank;

        if (!colleagueProjectileResourceDict.ContainsKey(colleagueInfo.colleagueType))
        {
            colleagueProjectileResourceDict.Add(colleagueInfo.colleagueType, new Dictionary<Rank, GameObject>());
        }

        if (!colleagueProjectileResourceDict[colleagueType].ContainsKey(rank))
        {
            colleagueProjectileResourceDict[colleagueType].Add(rank, projectile.gameObject);
        }

        colleagueProjectileResourceDict[colleagueType][rank] = projectile.gameObject;

        /*foreach (var projectile in projectiles)
        {
            if (!heroProjectileResourceDict.ContainsKey(projectile.index))
            {
                heroProjectileResourceDict.Add(projectile.index, projectile.gameObject);
            }
        }*/
    }

}