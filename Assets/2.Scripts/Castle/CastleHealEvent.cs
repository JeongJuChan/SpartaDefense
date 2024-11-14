using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleHealEvent
{
    private string castleHealEventId;
    private float healingCycleTime;
    private float interval;

    private float remainTime;

    private float healingAmount;

    public event Action<float> OnHealCastle;

    public CastleHealEvent(Castle castle, float healingCycleTime)
    {
        castle.OnStartHealEvent += StartHealEvent;
        castle.OnStopHealEvent += StopHealEvent;
        castle.OnSetCastleHealingAmount += SetHealingAmount;

        this.healingCycleTime = healingCycleTime;
        interval = healingCycleTime;
        remainTime = healingCycleTime;
    }

    private void StartHealEvent()
    {
        castleHealEventId = EventScheduler.instance.ScheduleRepeatingEvent(interval, HealCastle);
    }

    private void HealCastle()
    {
        remainTime -= interval;
        if (remainTime <= 0f)
        {
            remainTime = healingCycleTime;
            OnHealCastle?.Invoke(healingAmount);
        }
    }

    private void StopHealEvent()
    {
        EventScheduler.instance.StopRepeatingEvent(castleHealEventId);
    }

    private void SetHealingAmount(float healingAmount)
    {
        this.healingAmount = healingAmount;
    }
}
