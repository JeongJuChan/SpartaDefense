using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxButton : MonoBehaviour
{
    [SerializeField] private Button checkBoxButton;
    [SerializeField] private GameObject checkBox;

    public event Action<bool> OnUpdateCheckBoxState;

    private bool checkBoxState;

    public void Init()
    {
        checkBoxButton.onClick.AddListener(ToggleCheckBoxState);
    }

    public void ToggleCheckBoxState()
    {
        checkBoxState = !checkBoxState;
        checkBox.SetActive(checkBoxState);

        OnUpdateCheckBoxState?.Invoke(checkBoxState);
    }
}
