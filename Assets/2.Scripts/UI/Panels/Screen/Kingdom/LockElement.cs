using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockElement : MonoBehaviour
{
    [SerializeField] private FeatureID featureID;

    [SerializeField] private GameObject lockPanel;

    [SerializeField] private Button elementButton;

    public event Action OnButtonClicked;

    private bool isLocked;

    private UI_Alert ui_Alert;

    private UnlockData unlockData;

    public void InitUnlock()
    {
        ui_Alert = UIManager.instance.GetUIElement<UI_Alert>();

        elementButton.onClick.AddListener(OnClickButton);

        isLocked = ES3.Load($"{name}{Consts.IS_LOCKED}", true, ES3.settings);

        if (isLocked)
        {
            unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);

            if (unlockData == null)
            {
                return;
            }

            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count, () =>
            {
                isLocked = false;

                ES3.Save($"{name}{Consts.IS_LOCKED}", isLocked, ES3.settings);

                ES3.StoreCachedFile();
                lockPanel.SetActive(isLocked);
            }));
        }

        lockPanel.SetActive(isLocked);
    }

    private void OnClickButton()
    {
        if (!isLocked)
        {
            OnButtonClicked?.Invoke();
            return;
        }

        if (unlockData == null)
        {
            ui_Alert.AlertMessage($"<color=green>추후에 업데이트 될 컨텐츠입니다.</color>");
            return;
        }

        UnlockManager.Instance.ToastLockMessage(unlockData.featureType, unlockData.count);

        /*Debug.Log("This element is locked.");
        if (unlockData.featureType == FeatureType.Level)
        {
            ui_Alert.AlertMessage($"캐릭터 레벨 <color=red>{unlockData.count}</color>을 달성해야 합니다.");
        }
        else if (unlockData.featureType == FeatureType.Stage)
        {
            ui_Alert.AlertMessage($"<color=red>{EnumToKR.TransformStageNumber(unlockData.count)}</color>을 클리어 해야합니다.");
        }
        else if (unlockData.featureType == FeatureType.Quest)
        {

            ui_Alert.AlertMessage($"퀘스트 <color=red>{QuestManager.instance.GetTargetDescription(unlockData.count)}</color>을 클리어 해야합니다.");
        }*/
    }
}
