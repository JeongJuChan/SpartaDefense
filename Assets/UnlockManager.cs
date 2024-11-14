using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    private List<UnlockableFeature> features = new List<UnlockableFeature>();
    private Dictionary<FeatureType, Func<int, bool>> featureDict = new Dictionary<FeatureType, Func<int, bool>>();

    public event Action<FeatureID> OnUpdateUnlockUI;

    private UI_Alert uiAlert;

    public void Init()
    {
        OnUpdateUnlockUI = null;
        features.Clear();
        featureDict.Clear();
    }

    public void RegisterFeature(UnlockableFeature feature)
    {
        feature.SetUnlockCondition(featureDict[feature.Type]);
        features.Add(feature);
    }

    public void SetUnlockCondition(FeatureType featureType, Func<int, bool> condition)
    {
        featureDict[featureType] = condition;
    }

    public void ToastLockMessage(FeatureType featureType, int count)
    {
        if (uiAlert == null)
        {
            uiAlert = UIManager.instance.GetUIElement<UI_Alert>();
        }

        switch (featureType)
        {
            case FeatureType.Level:
                uiAlert.AlertMessage($"캐릭터 레벨 <color=green>{count}</color>을 달성해야 합니다.");
                break;
            case FeatureType.Stage:
                uiAlert.AlertMessage($"<color=green>{Difficulty.TransformStageNumber(count)}</color>을 클리어 해야 합니다.");
                break;
            case FeatureType.Quest:
                uiAlert.AlertMessage($"퀘스트 <color=green>{count - 1}</color>을 클리어 해야 합니다.");
                break;
            case FeatureType.BarracksLevel:
                uiAlert.AlertMessage($"병영 레벨 <color=green>{count}</color>을 달성해야 합니다.");
                break;
            case FeatureType.Dialogue:
                // TOOD: 임시
                uiAlert.AlertMessage($"튜토리얼 <color=green>장비 판매</color>를 달성해야 합니다.");
                break;
        }
    }

    // Updated CheckUnlocks to only check features of a specific type
    public void CheckUnlocks(FeatureType featureType)
    {
        foreach (var feature in features)
        {
            if (feature.Type == featureType && !feature.IsUnlocked && feature.UnlockCondition(feature.Count))
            {
                feature.IsUnlocked = true;
                NotifyUnlock(feature);
            }
        }
    }

    private void NotifyUnlock(UnlockableFeature feature)
    {
        feature.OnUnlockAction?.Invoke();
        OnUpdateUnlockUI?.Invoke(feature.ID);
        Debug.Log($"Unlocked: {feature.ID}");
    }
}