using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private SkillDataSO skillDataSO;
    [SerializeField] private SkillIncrementDataSO skillIncrementDataSO;

    public static Action<int, int, bool> OnUpdateSkillEquipUI;
    public event Func<List<Monster>> OnGetActiveTargets;
    public event Action<ColleagueType> OnPlayUsingSkillParticle;

    private Func<int, SkillData> OnGetSkillData;
    private Func<int, SkillResourceData> OnGetSkillResourceData;

    private Dictionary<int, Queue<Skill>> skillDict = new Dictionary<int, Queue<Skill>>();

    private BattleManager battleManager;

    private List<Monster> monsters = new List<Monster>();

    [SerializeField] private Transform wallTransform;
    private float wallTriggerPosX;

    private SkillUpgradeDataHandler skillUpgradeDataHandler;

    private ColleagueManager colleagueManager;
    
    public void Init()
    {
        OnUpdateSkillEquipUI = null;
        OnGetSkillData += skillDataSO.GetSkillData;
        OnGetSkillResourceData += ResourceManager.instance.skill.GetSkillResourceData;
        battleManager = FindAnyObjectByType<BattleManager>();
        skillUpgradeDataHandler = new SkillUpgradeDataHandler(skillIncrementDataSO.GetSkillIncrementData, skillDataSO.GetSkillData);
        colleagueManager = FindAnyObjectByType<ColleagueManager>();
        colleagueManager.OnColleagueAdvanced += skillUpgradeDataHandler.UpdateSkillUpgradableData;
        colleagueManager.OnGetSkillUpgradableData += skillUpgradeDataHandler.GetSkillUpgradableData;
        colleagueManager.OnUpdateSkillUpgradableData += skillUpgradeDataHandler.UpdateSkillUpgradableData;
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.EquipColleagueAuto, () => { QuestManager.instance.UpdateCount(EventQuestType.EquipColleagueAuto, PlayerPrefs.HasKey("EquipSkillAuto") ? 1 : 0, -1); });
        skillUpgradeDataHandler.OnUpdateColleagueUIPopup += UIManager.instance.GetUIElement<UI_Colleague>().UpdateColleaguePopup;
    }

    public SkillUpgradableData GetSkillUpgradableData(int index)
    {
        return skillUpgradeDataHandler.GetSkillUpgradableData(index);
    }

    public void UpdateSkillUpgradableData(int index)
    {
        skillUpgradeDataHandler.UpdateSkillUpgradableData(index);
    }

    public void OnSkillSummoned(int index, int count, bool isLastSkill)
    {
        OnUpdateSkillEquipUI.Invoke(index, count, isLastSkill);
    }

    public float GetSkillCoolTime(int index)
    {
        return OnGetSkillData.Invoke(index).skillUpgradableData.coolTime;
    }

    public float UseSkill(ColleagueType colleagueType, int skillIndex)
    {
        BigInteger damage = StatDataHandler.Instance.GetDamage(colleagueType);

        SkillData skillData = OnGetSkillData.Invoke(skillIndex);
        monsters.Clear();

        skillData.skillUpgradableData = skillUpgradeDataHandler.GetSkillUpgradableData(skillIndex);
        monsters.AddRange(OnGetActiveTargets.Invoke());

        if (monsters.Count == 0)
        {
            return 0f;
        }

        SkillResourceData skillResourceData = OnGetSkillResourceData.Invoke(skillIndex);

        int skillCount = skillData.skillUpgradableData.targetCount;

        if (!skillDict.ContainsKey(skillIndex))
        {
            Queue<Skill> skills = new Queue<Skill>(skillCount);
            for (int i = 0; i < skillData.skillUpgradableData.targetCount && i < monsters.Count; i++)
            {
                Skill skill = InstantiateSkill(skillResourceData);
                skill.SetIndex(skillIndex);
                skill.gameObject.SetActive(false);
                skills.Enqueue(skill);
            }

            skillDict.Add(skillIndex, skills);
        }
        else
        {
            if (skillDict[skillIndex].Count < monsters.Count && skillDict[skillIndex].Count < skillCount)
            {
                for (int i = 0; i < skillCount - skillDict[skillIndex].Count; i++)
                {
                    Skill skill = InstantiateSkill(skillResourceData);
                    skill.gameObject.SetActive(false);
                    skillDict[skillIndex].Enqueue(skill);
                }
            }
        }

        int layerIndex = 0;
        float duration = 0f;

        if (skillDict[skillIndex].Peek().TryGetComponent(out ISkillDrop skillDrop))
        {
            if (wallTransform != null && wallTriggerPosX == 0f)
            {
                float halfWidth = wallTransform.GetComponent<BoxCollider2D>().size.x * Consts.HALF;
                float wallPosAbs = wallTransform.position.x < 0 ? -wallTransform.position.x : wallTransform.position.x;
                wallTriggerPosX = wallPosAbs + halfWidth;
            }

            skillDrop.SetWallTriggerPosX(wallTriggerPosX);

            if (monsters.Count == 1)
            {
                duration = UseDropSkill(skillIndex, monsters[0], skillData, layerIndex, duration, damage);
            }
            else
            {
                monsters.RemoveAll((monster) =>
                {
                    if (skillCount <= 0)
                    {
                        return false;
                    }

                    duration = UseDropSkill(skillIndex, monster, skillData, layerIndex, duration, damage);
                    skillCount--;
                    return true;
                });
            }

            OnPlayUsingSkillParticle?.Invoke(colleagueType);
            return duration;
        }


        if (monsters.Count == 1)
        {
            duration = UseNormalSkill(skillIndex, monsters[0], skillData, layerIndex, duration, damage);
        }
        else
        {

            monsters.Sort((x, y) => -x.transform.position.x.CompareTo(y.transform.position.x));

            monsters.RemoveAll((monster) =>
            {
                if (skillCount <= 0)
                {
                    return false;
                }

                duration = UseNormalSkill(skillIndex, monster, skillData, layerIndex, duration, damage);
                skillCount--;
                return true;
            });
        }

        OnPlayUsingSkillParticle?.Invoke(colleagueType);
        return duration;
    }

    private float UseDropSkill(int index, Monster monster, SkillData skillData, int layerIndex, float duration, BigInteger damage)
    {
        if (skillDict[index].Count <= 0)
        {
            return 0f;
        }

        Skill skill = skillDict[index].Dequeue();
        skill.Use(skillData, monster, damage);

        if (skill.TryGetComponent(out ISkillAniamted skillAniamted))
        {
            duration = skillAniamted.GetAnimationDuraiton(layerIndex);
        }

        skillDict[index].Enqueue(skill);

        return duration;
    }

    private float UseNormalSkill(int index, Monster monster, SkillData skillData, int layerIndex, float duration, BigInteger damage)
    {
        if (skillDict[index].Count <= 0)
        {
            return 0f;
        }

        Skill skill = skillDict[index].Dequeue();
        skill.Use(skillData, monster, damage);

        if (skill.TryGetComponent(out ISkillAniamted skillAniamted))
        {
            duration = skillAniamted.GetAnimationDuraiton(layerIndex);
            skillAniamted.DeActivateAsAnimationFinished(layerIndex);
        }

        skillDict[index].Enqueue(skill);
        return duration;
    }

    private Skill InstantiateSkill(SkillResourceData skillResourceData)
    {
        Skill skill = Instantiate(skillResourceData.skill, transform);
        skill.OnDamageTarget += battleManager.OnMonsterAttacked;
        return skill;
    }

    /*private void OnUpdateSkillUpgradableData(int index, SkillUpgradableData skillUpgradableData)
    {
        if (!skillUpgradableDataDict.ContainsKey(index))
        {
            skillUpgradableDataDict.Add(index, skillUpgradableData);
        }

        skillUpgradableDataDict[index] = skillUpgradableData;
    }*/

    

#if UNITY_EDITOR
    public void EditorGetSkill(int index, int count)
    {
        OnSkillSummoned(index, count, true);
    }

#endif
}