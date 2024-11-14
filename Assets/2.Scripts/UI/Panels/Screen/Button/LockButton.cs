using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockButton : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private Image refImage;
    [SerializeField] private FeatureID featureID;

    public void Init()
    {
        GetComponent<Image>().sprite = refImage.sprite;

        UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);

        UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count,
            () => gameObject.SetActive(false)));
    }

}
