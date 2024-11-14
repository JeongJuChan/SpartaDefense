using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummonResultSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private Image iconShadow;
    [SerializeField] private Image background;
    [SerializeField] private Image showEffect;
    [SerializeField] private Image showRankEffect;
    [SerializeField] private TextMeshProUGUI rankText;

    protected Rank rank;
    public event Action OnAnimEnd;

    protected Sequence scaleAnim;
    protected Sequence scaleAnimWithCallback;
    protected Sequence fadeAnim;
    protected Coroutine effectCoroutine;

    protected WaitForSeconds waitForHideEffect = new WaitForSeconds(.01f);
    protected WaitForSeconds waitForSlotInfo = new WaitForSeconds(0.7f);

    [SerializeField] protected GameObject slotInfo;
    protected Rank effectStartGrade = Rank.Rare;
    // [SerializeField] protected ParticleSystem hideParticle;
    // [SerializeField] protected ParticleSystem[] gradeParticles;

    UIAnimations uIAnimations;

    public void InitSlot(UIAnimations uIAnimations)
    {
        this.uIAnimations = uIAnimations;
    }

    public void SetSlotInfo(ISummonable item)
    {
        if (item is EquipmentData)
        {
            EquipmentData equipment = item as EquipmentData;

            icon.sprite = EquipmentManager.instance.GetIcon(equipment);
            iconShadow.sprite = EquipmentManager.instance.GetIcon(equipment);
            rank = equipment.rank;
            background.sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(rank);
            rankText.text = EnumToKRManager.instance.GetEnumToKR(rank);
            // rankText.color = ResourceManager.instance.rank.GetRankColor(equipment.rank);
        }
        else if (item is SkillData)
        {
            SkillData skill = (SkillData)item;

            icon.sprite = ResourceManager.instance.skill.GetSkillResourceData(skill.index).skillIcon;
            iconShadow.sprite = ResourceManager.instance.skill.GetSkillResourceData(skill.index).skillIcon;

            rank = skill.rank;
            background.color = ResourceManager.instance.rank.GetRankColor(skill.rank);
            background.sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(rank);
            rankText.text = EnumToKRManager.instance.GetEnumToKR(rank);

            // rankText.color = ResourceManager.instance.rank.GetRankColor(skill.rank);
        }
        else if (item is ColleagueData)
        {
            ColleagueData colleagueData = (ColleagueData)item;

            icon.sprite = ResourceManager.instance.slotHeroData.GetResource(colleagueData.colleagueInfo).defaultSprite;
            iconShadow.sprite = ResourceManager.instance.slotHeroData.GetResource(colleagueData.colleagueInfo).defaultSprite;
            rank = colleagueData.colleagueInfo.rank;

            background.sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(rank);
            rankText.text = EnumToKRManager.instance.GetEnumToKR(rank);
            // rankText.color = ResourceManager.instance.rank.GetRankColor(skill.rank);
        }

        slotInfo.SetActive(false);
    }

    public bool ShowSlot(bool isOnSkipMode)
    {
        // if (fadeAnim == null) fadeAnim = UIAnimations.FadeIn(slotInfo);
        if (scaleAnim == null) scaleAnim = uIAnimations.ScaleUp(this.gameObject).SetAutoKill(false);
        if (scaleAnimWithCallback == null) scaleAnimWithCallback = uIAnimations.ScaleUp(this.gameObject, () => fadeAnim.Restart());

        gameObject.SetActive(true);

        if (rank <= effectStartGrade && !isOnSkipMode)
        {
            scaleAnim.Restart();
            HideSlot();
            uIAnimations.ShowRankEffect(showRankEffect, rank);
            return false;
        }
        else
        {
            Debug.Log("slowSlot : " + rank);
            scaleAnim.Restart();
            HideSlot();
            uIAnimations.ShowRankEffect(showRankEffect, rank);
            uIAnimations.ShakeSummonResult();
            return true;
        }
    }

    private void HideSlot()
    {
        uIAnimations.Hide(showEffect);
    }


    public void ResetSlot()
    {
        transform.localScale = Vector3.zero;
        scaleAnim = null;
        scaleAnimWithCallback = null;
        slotInfo.SetActive(false);
        gameObject.SetActive(false);
    }

    protected void AnimEnd()
    {
        OnAnimEnd?.Invoke();
    }
}
