using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Kingdom : UI_BottomElement
{
    [SerializeField] private GuideController guideController;
    [SerializeField] private GameObject kingdomChoiceUIPanel;

    [SerializeField] private BarracksUI barracksUI;
    [SerializeField] private AbilityUI laboratoryUI;

    [SerializeField] private LockElement barracksElement;
    [SerializeField] private LockElement laboratoryElement;
    [SerializeField] private LockElement kingsRoadElement;
    [SerializeField] private LockElement doorElement;
    [SerializeField] private LockElement castleBattleElement;

    public override void Initialize()
    {
        base.Initialize();
        barracksUI.Init();
        guideController.Initialize();
        laboratoryUI.Init();

        barracksElement.InitUnlock();
        barracksElement.OnButtonClicked += OpenBarracksUI;
        laboratoryElement.InitUnlock();
        laboratoryElement.OnButtonClicked += OpenLaboratoryUI;
        kingsRoadElement.InitUnlock();
        kingsRoadElement.OnButtonClicked += OpenKingsRoadUI;
        doorElement.InitUnlock();
        doorElement.OnButtonClicked += OpenDoorUI;
        castleBattleElement.InitUnlock();
        castleBattleElement.OnButtonClicked += OpenCastleBattleUI;

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.TouchBarracks,
            () => QuestManager.instance.UpdateCount(EventQuestType.TouchBarracks, PlayerPrefs.HasKey(Consts.TOUCH_BARRACKS) ? 1 : 0, -1));
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.Training, 
            () => QuestManager.instance.UpdateCount(EventQuestType.TouchBarracks, 0, -1));
    }

    public override void StartInit()
    {
        laboratoryUI.StartInit();
    }

    private void OpenBarracksUI()
    {
        barracksUI.ActivateSelf(true);
        kingdomChoiceUIPanel.SetActive(false);
    }

    private void OpenLaboratoryUI()
    {
        laboratoryUI.ActivateSelf(true);
        // kingdomChoiceUIPanel.SetActive(false);
        // laboratoryLockElement.ActivateSelf(true);
    }

    private void OpenKingsRoadUI()
    {
        kingdomChoiceUIPanel.SetActive(false);
    }

    private void OpenDoorUI()
    {
        kingdomChoiceUIPanel.SetActive(false);
    }

    private void OpenCastleBattleUI()
    {
        kingdomChoiceUIPanel.SetActive(false);
    }
}
