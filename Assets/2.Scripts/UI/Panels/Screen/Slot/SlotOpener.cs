using System;
using System.Collections.Generic;
using UnityEngine;
public class SlotOpener : MonoBehaviour
{
    [SerializeField] private UnlockDataSO unlockDataSO;
    private SlotEquipmentForger slotEquipmentForger;
    private SlotEvent slotEvent = new SlotEvent();
    private StageController stageController;
    private Func<int, UnlockSlotByStageData> OnGetUnlockSlotData;
    public event Action OnOpenSkillSlot;

    public Func<List<UnlockSlotByStageData>> OnGetUnlockSlotByStageDatas;
    public Func<int> OnGetOpenSkillSlotIndex;
    private Func<int> OnGetOpenSlotCount;

    public int forgeSlotCount { get; private set; } = 1;

    private ColleagueType[] slotTypes;

    public void Init()
    {
        /*stageController = FindAnyObjectByType<StageController>();
        //OnGetUnlockSlotData += unlockSlotByStageDataSO.GetUnlockSlotData;
        //OnGetUnlockSlotByStageDatas += unlockSlotByStageDataSO.GetUnlockSlotDatas;
        slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>();
        OnGetOpenSlotCount += slotEquipmentForger.slotStatRoller.GetOpenSlotCount;
        slotEvent.OnOpenSlot += slotEquipmentForger.slotStatRoller.AddSlotType;
        slotEvent.OnOpenSlot += ResourceManager.instance.slotHeroData.LoadSlotTypeProjectiles;
        //slotEvent.CallOpenSlot(ColleagueType.Rtan);
        stageController.OnUpdateStageIndex += TryOpenSlots;

        slotTypes = (ColleagueType[])Enum.GetValues(typeof(ColleagueType));

        foreach (UnlockData data in unlockDataSO.GetForgeUnlockDatas())
        {
            string[] featureIDStrSplit = data.featureID.ToString().Split('_');
            int slotNum = int.Parse(featureIDStrSplit[1]);
            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(data.featureType, data.featureID, data.count, () => UnlockForgeSlot(slotNum)));
        }

        foreach (UnlockData data in unlockDataSO.GetSkillUnlockDatas())
        {
            string[] featureIDStrSplit = data.featureID.ToString().Split('_');
            int slotNum = int.Parse(featureIDStrSplit[1]);
            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(data.featureType, data.featureID, data.count, () => UnlockSkillSlot(slotNum)));
        }

        LoadDatas();*/
    }

    private void TryOpenSlots(int index)
    {
        //UnlockSlotByStageData unlockSlotByStageData = OnGetUnlockSlotData.Invoke(index);
        //UnlockSlots(unlockSlotByStageData);
    }

    private void UnlockForgeSlot(int forgeNum)
    {
        int openSlotCount = OnGetOpenSlotCount.Invoke();
        openSlotCount = openSlotCount == 0 ? openSlotCount + 2 : openSlotCount + 1;

        if (forgeNum == 0)
        {

            for (int i = openSlotCount; i <= forgeSlotCount; i++)
            {
                slotEvent.CallOpenSlot(slotTypes[i]);
            }
        }
        else
        {
            if (forgeSlotCount <= forgeNum)
            {
                for (int i = openSlotCount; i <= forgeNum; i++)
                {
                    slotEvent.CallOpenSlot(slotTypes[i]);
                }

                if (forgeNum > forgeSlotCount)
                {
                    forgeSlotCount = forgeNum;
                }
            }
        }

        //SaveDatas();
    }

    /*private void UnlockSkillSlot(int skillNum)
    {
        if (skillNum == 0)
        {
            for (int i = OnGetOpenSkillSlotIndex.Invoke(); i < skillSlotCount; i++)
            {
                OnOpenSkillSlot?.Invoke();
            }
        }
        else
        {
            if (OnGetOpenSkillSlotIndex == null)
            {
                for (int i = 0; i < skillNum; i++)
                {
                    OnOpenSkillSlot?.Invoke();
                }

                return;
            }

            for (int i = OnGetOpenSkillSlotIndex.Invoke(); i < skillNum; i++)
            {
                OnOpenSkillSlot?.Invoke();
            }

            if (skillNum > skillSlotCount)
            {
                skillSlotCount = skillNum;
            }
        }

        SaveDatas();
    }*/

    /*private void UnlockSlots(UnlockSlotByStageData unlockSlotByStageData)
    {
        int forgeNum = unlockSlotByStageData.forgeNum;
        int skillNum = unlockSlotByStageData.skillNum;

        int openSlotCount = OnGetOpenSlotCount.Invoke();
        openSlotCount = openSlotCount == 0 ? openSlotCount + 2 : openSlotCount + 1;

        if (forgeNum == 0)
        {
            
            for (int i = openSlotCount; i <= forgeSlotCount; i++)
            {
                slotEvent.CallOpenSlot(slotTypes[i]);
            }
        }
        else
        {
            if (forgeSlotCount <= forgeNum)
            {
                for (int i = openSlotCount; i <= forgeNum; i++)
                {
                    slotEvent.CallOpenSlot(slotTypes[i]);
                }

                if (forgeNum > forgeSlotCount)
                {
                    forgeSlotCount = forgeNum;
                }
            }
        }

        if (skillNum == 0)
        {
            for (int i = OnGetOpenSkillSlotIndex.Invoke(); i < skillSlotCount; i++)
            {
                OnOpenSkillSlot?.Invoke();
            }
        }
        else
        {
            if (OnGetOpenSkillSlotIndex == null)
            {
                for (int i = 0; i < skillNum; i++)
                {
                    OnOpenSkillSlot?.Invoke();
                }

                return;
            }

            for (int i = OnGetOpenSkillSlotIndex.Invoke(); i < skillNum; i++)
            {
                OnOpenSkillSlot?.Invoke();
            }

            if (skillNum > skillSlotCount)
            {
                skillSlotCount = skillNum;
            }
        }

        SaveDatas();
    }*/


    /*public void SaveDatas()
    {
        ES3.Save(Consts.FORGE_SLOT_COUNT, forgeSlotCount, ES3.settings);
        ES3.Save(Consts.COLLEAGUESLOTINDEX, skillSlotCount, ES3.settings);

        ES3.StoreCachedFile();
    }

    public void LoadDatas()
    {
        if (ES3.KeyExists(Consts.FORGE_SLOT_COUNT))
        {
            forgeSlotCount = ES3.Load<int>(Consts.FORGE_SLOT_COUNT);
        }
        if (ES3.KeyExists(Consts.COLLEAGUESLOTINDEX))
        {
            skillSlotCount = ES3.Load<int>(Consts.COLLEAGUESLOTINDEX);
        }
    }*/

#if UNITY_EDITOR
    public void EditorOpenSlot(ColleagueType slotType)
    {
        slotEvent.CallOpenSlot(slotType);
    }
    public void EditorOpenSkillSlot()
    {
        for (int i = 0; i < 6; i++)
        {
            OnOpenSkillSlot?.Invoke();
        }
    }
#endif
}