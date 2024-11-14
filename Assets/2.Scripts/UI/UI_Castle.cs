using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Castle : UI_BottomElement
{
    [SerializeField] CastleProgressionInfoPanel castleProgressionInfoPanel;

    public override void Initialize()
    {
        base.Initialize();
        castleProgressionInfoPanel.Init();
        FindAnyObjectByType<StageController>().OnUpdateCastleQuest += castleProgressionInfoPanel.SetCastleQuestUIs;
    }

    public override void StartInit()
    {
        UpdateCastleQuestUI();
        castleProgressionInfoPanel.StartInit();
    }

    public void UpdateCastleQuestUI()
    {
        castleProgressionInfoPanel.SetCastleQuestUIs();
    }
}
