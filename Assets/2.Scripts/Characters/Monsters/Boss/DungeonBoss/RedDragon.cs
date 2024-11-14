using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragon : Monster
{
    private readonly int ATTACK_INDEX = AnimatorParameters.ATTACK_INDEX;

    private const string ATTACK_STATE_TAG = "Attack";

    [SerializeField] private int attackMaxIndexExcluded = 3;

    protected override void PlayAttackAnimation(bool isPlaying)
    {
        base.PlayAttackAnimation(isPlaying);

        StartCoroutine(PlayRandomAttackAnimation());
    }

    private IEnumerator PlayRandomAttackAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsTag(ATTACK_STATE_TAG))
        {
            yield return null;
        }

        int index = UnityEngine.Random.Range(0, attackMaxIndexExcluded);

        animator.SetInteger(ATTACK_INDEX, index);
    }
}
