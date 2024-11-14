using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rtan : ParabolaShooter
{
    public event Func<int, float> OnGetAttributeValue;
    private Dictionary<int, float> rtanAttributeDict = new Dictionary<int, float>();

    private int offsetProjectileCount;

    private const float PERCENT = Consts.PERCENT_DIVIDE_VALUE;

    private const int MULTI_SHOT_COUNT = 2;
    private const int TRIPLE_SHOT_COUNT = 3;

    public Func<Dictionary<AttributeType, int>> OnGetAttributeIndexDict;

    public Func<AttributeType, int> OnGetAttributeIndex;

    protected override void Awake()
    {
        base.Awake();
        offsetProjectileCount = projectileCount;
    }

    protected override void Shoot()
    {
        //projectileCount = GetProjectileCount();

        base.Shoot();
    }

    /*private int GetProjectileCount()
    {
        float multiShotRandom = UnityEngine.Random.Range(0, PERCENT);
        float multiShotValue = OnGetAttributeValue.Invoke(OnGetAttributeIndex.Invoke(AttributeType.MultiShot));
        bool canMultiShot = multiShotRandom < multiShotValue;

        float tripleShotValue = OnGetAttributeValue.Invoke(OnGetAttributeIndex.Invoke(AttributeType.TripleShot));
        float tripleShotRandom = UnityEngine.Random.Range(0, PERCENT);
        bool canTripleShot = tripleShotRandom < tripleShotValue;

        if (canMultiShot)
        {
            if (canTripleShot)
            {
                return TRIPLE_SHOT_COUNT;
            }

            return MULTI_SHOT_COUNT;
        }

        if (canTripleShot)
        {
            return TRIPLE_SHOT_COUNT;
        }

        return offsetProjectileCount;
    }*/
}
