﻿using System.Collections.Generic;
using System;
using UnityEngine;

public class EffectDataHandler
{
    private Dictionary<int, GameObject> effectResourceDict = new Dictionary<int, GameObject>();
    private const string Hit_EFFECT_PATH = "Effects/";
    
    private Func<List<EffectData>> OnGetAllEffectData;

    public EffectDataHandler(Func<List<EffectData>> OnGetAllEffectData)
    {
        this.OnGetAllEffectData = OnGetAllEffectData;
        LoadEffect();
    }

    public GameObject GetResource(int index)
    {
        if (effectResourceDict.ContainsKey(index))
        {
            return effectResourceDict[index];
        }

        return null;
    }

    private void LoadEffect()
    {
        List<EffectData> effectDatas = OnGetAllEffectData.Invoke();

        foreach (EffectData effectData in effectDatas)
        {
            Effect effect = Resources.Load<Effect>($"{Hit_EFFECT_PATH}{effectData.name}");
            effect.SetIndex(effectData.index);
            if (!effectResourceDict.ContainsKey(effectData.index))
            {
                effectResourceDict.Add(effectData.index, effect.gameObject);
            }
        }
    }
}