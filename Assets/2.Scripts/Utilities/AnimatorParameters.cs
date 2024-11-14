using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameters
{
    [Header("Common")]
    public static readonly int IS_ATTACK_HASH = Animator.StringToHash("IsAttacking");
    public static readonly int ATTACK_SPEED_RATE_HASH = Animator.StringToHash("AttackSpeedRate");

    [Header("Monster")]
    public static readonly int IS_STUNNED_HASH = Animator.StringToHash("IsStunned");
    public static readonly int IS_DEAD_HASH = Animator.StringToHash("IsDead");

    [Header("Boss")]
    public static readonly int ATTACK_INDEX = Animator.StringToHash("AttackIndex");

    [Header("Skill")]
    public static readonly int IS_TRIGGER_HASH = Animator.StringToHash("IsTrigger");

    [Header("ColleagueUI")]
    public const string DEFAULT_LAYER_NAME = "Base Layer.";

    [Header("AutoForge")]
    public const string IS_AUTO_ON = "IsAutoOn";
}
