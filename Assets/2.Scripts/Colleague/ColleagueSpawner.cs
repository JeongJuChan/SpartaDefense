using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColleagueSpawner : MonoBehaviour
{
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private ColleagueDataSO colleagueDataSO;
    [SerializeField] private float rtanSkillParticlePosY = -6.5f;
    [SerializeField] private float otherSkillParticlePosY = -5f;

    public event Action<Hero> OnHeroInstantiated;

    private Castle castle;

    private const string FIRE_PIVOT_NAME = "FirePivot";


    public void Init()
    {
        castle = FindAnyObjectByType<Castle>();
    }

    public void InstantiateHero(int index)
    {
        Hero hero;
        
        ColleagueData colleagueData = colleagueDataSO.GetColleagueData(index);
        ShootingType shootingType = colleagueData.shootingType;
        if (shootingType == ShootingType.Parabola)
        {
            hero = Instantiate(heroPrefab).AddComponent<ParabolaShooter>();
        }
        else
        {
            hero = Instantiate(heroPrefab).AddComponent<StraightShooter>();
        }

        castle.OnDead += hero.ResetTarget;

        foreach (Transform transform in hero.transform)
        {
            if (transform.name == FIRE_PIVOT_NAME)
            {
                hero.SetFirePivot(transform);
            }
        }


        //hero.SetSlotInfo(colleagueType);
        hero.SetColleagueData(colleagueData);

        hero.SetUsingSkillParticleColor();

        if (colleagueData.colleagueInfo.colleagueType == ColleagueType.Rtan_Rare)
        {
            hero.SetUsingSkillParticlePosY(rtanSkillParticlePosY);
        }
        else
        {
            hero.SetUsingSkillParticlePosY(otherSkillParticlePosY);
        }

        OnHeroInstantiated?.Invoke(hero);
    }
}
