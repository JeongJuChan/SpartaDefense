using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotEquipmentForger : MonoBehaviour
{
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private Castle castle;

    public event Func<int> OnGetCastleLevel;

    public event Action<Hero> OnHeroInstantiated;

    public event Func<Rank> OnGetAutoMinimumRank;
    public event Action<bool> OnActiveForgePopup;
    public event Action<bool> OnContinueAuto;

    public event Func<bool> OnGetAutoForgeStopped;

    public event Action<EquipmentType, Color, Sprite> OnUpdateForgePreImage;

    // TODO: Replace Real aniamtion duration;
    [SerializeField] private ParticleSystem afterForgeParticle;
    [SerializeField] private float showingPopupWaitDuration = 0.25f;
    [SerializeField] private float showingSlotImageWaitPercent = 0f;
    [SerializeField] private float showingPopupDelayDuration = 0.5f;
    [SerializeField] private int vibrateDuration = 50;


    private WaitForSeconds forgeWaitForSeconds;
    private WaitForSeconds particleWaitForSeconds;
    private WaitForSeconds showingPopupDelayWaitForSeconds;
    private WaitForSeconds showingPopupWaitForSeconds;

    public event Func<EquipmentType> OnGetRandomEquipmentType;
    public event Func<EquipmentType, bool> OnGetIsSlotTypeInstantiated;
    public event Action<EquipmentType, int, BigInteger, BigInteger> OnSellNewSlotDelay;
    public event Func<int, Rank> OnGetRandomRank;
    public event Func<ForgeEquipmentInfo, SlotStatData> OnGetForgeStatData;
    //public event Func<Rank, Dictionary<int, float>> OnGetAttributeTypes;

    private ForgePreImagePanel forgePreImagePanel;

    public void Init()
    {
        float particleDuration = afterForgeParticle.main.startLifetime.constant * afterForgeParticle.main.duration;
        particleWaitForSeconds = CoroutineUtility.GetWaitForSeconds(particleDuration);
        showingPopupWaitForSeconds = CoroutineUtility.GetWaitForSeconds(showingPopupWaitDuration);

        showingPopupDelayWaitForSeconds = CoroutineUtility.GetWaitForSeconds(showingPopupDelayDuration);

        forgePreImagePanel = UIManager.instance.GetUIElement<ForgePreImagePanel>();
        forgePreImagePanel.Init(showingPopupWaitDuration, showingSlotImageWaitPercent);
    }

    public void UpdateForgeAnimationDuration(float duration)
    {
        if (forgeWaitForSeconds == null || forgeWaitForSeconds != CoroutineUtility.GetWaitForSeconds(duration))
        {
            forgeWaitForSeconds = CoroutineUtility.GetWaitForSeconds(duration);
        }
    }

    public void StartForgeEffect(ForgeEquipmentData forgeEquipmentData, SlotStatData slotStatData, Sprite mainSprite)
    {
        StartCoroutine(WaitForForgeEffect(forgeEquipmentData, slotStatData, mainSprite));
    }

    public void ShowForgePreImageRightAway(ForgeEquipmentData forgeEquipmentData, Sprite mainSprite)
    {
        forgePreImagePanel.UpdatePreImageRightAway(
            ResourceManager.instance.rank.GetRankColor(forgeEquipmentData.forgeEquipmentInfo.rank), mainSprite);
    }

    private IEnumerator WaitForForgeEffect(ForgeEquipmentData forgeEquipmentData, SlotStatData slotStatData, Sprite mainSprite)
    {
        EquipmentType equipmentType = forgeEquipmentData.forgeEquipmentInfo.equipmentType;
        Rank rank = forgeEquipmentData.forgeEquipmentInfo.rank;

        yield return forgeWaitForSeconds;

        if (!UIManager.instance.isPopupOpened)
        {
            afterForgeParticle.Play();
        }

        Vibration.Vibrate(vibrateDuration);
        /*if (!castle.GetIsCastleDangerous())
        {
        }*/

        yield return particleWaitForSeconds;

        forgePreImagePanel.UpdatePreImage(ResourceManager.instance.rank.GetRankColor(rank), mainSprite);

        yield return showingPopupWaitForSeconds;

        yield return showingPopupDelayWaitForSeconds;

        bool isAuto = !OnGetAutoForgeStopped.Invoke();

        if (isAuto)
        {
            if (!OnGetIsSlotTypeInstantiated.Invoke(equipmentType))
            {
                OnActiveForgePopup?.Invoke(isAuto);
                yield break;
            }

            Rank minimumRank = OnGetAutoMinimumRank.Invoke();
            if (rank >= minimumRank)
            {
                OnActiveForgePopup?.Invoke(isAuto);
            }
            else
            {
                OnSellNewSlotDelay?.Invoke(equipmentType, slotStatData.level, slotStatData.exp, slotStatData.gold);
            }
        }
        else
        {
            OnActiveForgePopup?.Invoke(isAuto);
        }
    }
}
