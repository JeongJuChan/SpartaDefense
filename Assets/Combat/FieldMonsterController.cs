using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMonsterController : MonoBehaviour
{
    private TargetInActiveRangeTrigger targetInRangeTrigger;

    public event Action<Monster> OnTargetAdded;
    public event Action<Monster> OnTargetRemoved;

    #region UnityMethods
    private void Awake()
    {
        targetInRangeTrigger = FindAnyObjectByType<TargetInActiveRangeTrigger>();
    }

    private void Start()
    {
        targetInRangeTrigger.OnTargetAdded += AddTarget;
    }

    private void OnDestroy()
    {
        targetInRangeTrigger.OnTargetAdded -= AddTarget;
    }
    #endregion

    #region FieldTargetEvent
    private void AddTarget(Monster monster)
    {
        monster.OnRemoveTargetAction += RemoveTarget;
        monster.ToggleInvincible(false);
        OnTargetAdded?.Invoke(monster);
    }

    private void RemoveTarget(Monster monster)
    {
        monster.OnRemoveTargetAction -= RemoveTarget;
        OnTargetRemoved?.Invoke(monster);
    }
    #endregion
}
