using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitMaskPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private GameObject bottomBarDisablePanel;
    [SerializeField] private GameObject guide;
    [SerializeField] private Button forgeButton;

    private bool isFirstInitiated = true;

    public void Init()
    {
        // FirstOpen이 저장되어있지 않으면 FirstOpen부터 Open
        /*if (!ES3.KeyExists(Consts.DIALOGUE_TYPE_FIRST_OPEN, ES3.settings))
        {
            DialogManager.instance.ShowDialog(DialogueType.FirstOpen);
            return;
        }
*/
        /*LoadDatas();

        CheckInit();*/
    }

    public void CheckInit()
    {

        if (isFirstInitiated)
        {
            InitFirstSetting();
        }
        else
        {
            OnClickForgeButton();
        }
    }

    private void InitFirstSetting()
    {
        gameObject.SetActive(true);
        guide.SetActive(true);
        bottomBarDisablePanel.SetActive(true);
        forgeButton.onClick.AddListener(OnClickForgeButton);
    }

    private void OnClickForgeButton()
    {
        SaveDatas();
        bottomBarDisablePanel.SetActive(false);
        guide.SetActive(false);
        forgeButton.onClick.RemoveListener(OnClickForgeButton);
        gameObject.SetActive(false);
    }

    private void SaveDatas()
    {
        ES3.Save<bool>(Consts.DIALOGUE_TYPE_FORGE_TOUCH, false, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        isFirstInitiated = ES3.Load<bool>(Consts.DIALOGUE_TYPE_FORGE_TOUCH, true, ES3.settings);
    }


}
