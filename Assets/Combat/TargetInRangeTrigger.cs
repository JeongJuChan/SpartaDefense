using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInRangeTrigger : MonoBehaviour
{
    public event Action<Monster> OnTargetAdded;

    protected BoxCollider2D boxCollider;

    #region UnityMethods
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (collision.TryGetComponent(out Monster monster))
        {
            OnTargetAdded?.Invoke(monster);
        }
    }
    #endregion
}
