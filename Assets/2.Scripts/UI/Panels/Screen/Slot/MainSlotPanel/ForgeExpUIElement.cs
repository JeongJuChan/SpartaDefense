using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeExpUIElement : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backwardImage;

    private void OnEnable()
    {
        // ActiveFillImage(false);
    }

    public void ActiveFillImage(bool isActive)
    {
        fillImage.gameObject.SetActive(isActive);
    }
}
