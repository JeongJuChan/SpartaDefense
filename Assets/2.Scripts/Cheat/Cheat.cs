using UnityEngine;

public class Cheat : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SkillDataSO skillDataSO;
    private Castle castle;
    private StageController stageController;
    private SlotEquipmentForger slotEquipmentForger;
    private SkillManager skillManager;
    private SlotOpener slotOpener;
    private CastleProgressionInfoPanel castleProgressionInfoPanel;

    private void OnEnable()
    {
        castle = FindAnyObjectByType<Castle>();
        stageController = FindAnyObjectByType<StageController>();
        slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>();
        skillManager = FindAnyObjectByType<SkillManager>();
        slotOpener = FindAnyObjectByType<SlotOpener>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            QuestManager.instance.QuestCompleate();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, 100000);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ForgeTicket, 10);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gem, 10000);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (castleProgressionInfoPanel == null)
            {
                castleProgressionInfoPanel = FindAnyObjectByType<CastleProgressionInfoPanel>(FindObjectsInactive.Include);
            }
            castleProgressionInfoPanel.EditorUpgradeCastle();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SkipCurrentRoutineStage();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ColleagueLevelUpStone, 100000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ColleagueLevelUpDungeonTicket, 5);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.AccelerationTicket, 10);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ColleagueSummonTicket, 100);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.M))
        {
            UIManager.instance.GetUIElement<UI_Colleague>().SummonColleague(1, 10000, true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Slash))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.AbilityPoint, 100);
        }

        
    }

    #region SkillCheat
    
    public void CheatOpenNextSkillSlot()
    {
        slotOpener.EditorOpenSkillSlot();
    }

    public void CheatGetSkill(int index, int count)
    {
        skillManager.EditorGetSkill(index, count);
    }

    public void CheatForgeAllSkillSlot(int count)
    {
        foreach (var data in skillDataSO.GetSkillDatas())
        {
            skillManager.EditorGetSkill(data.index, count);
        }
    }

    #endregion

    #region SlotCheat
    public void CheatOpenSlot(ColleagueType slotType)
    {
        slotOpener.EditorOpenSlot(slotType);
    }

    public void CheatForgeSlot(Rank rank, ColleagueType slotType)
    {
        slotOpener.EditorOpenSlot(slotType);
        ForgeManager.instance.EditorForgeSlotEquipment(rank, slotType);
    }

    #endregion

    #region StageCheat
    public void KillCastle()
    {
        castle.TakeDamage(int.MaxValue);
    }

    public void SkipCurrentRoutineStage()
    {
        stageController.EditorSkipCurrentRoutineStage();
    }

    public void SkipCurrentSubStage()
    {
        stageController.EditorSkipCurrentSubStage();
    }

    public void ChallengeBoss()
    {
        stageController.EditorChallengeBoss();
    }

    public void SkipCurrentMainStage()
    {
        stageController.EditorSkipCurrentMainStage();
    }
    #endregion
#endif
}
