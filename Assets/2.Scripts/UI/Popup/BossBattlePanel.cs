using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBattlePanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private RectTransform rtanRectTransform;
    [SerializeField] private RectTransform bossRectTransform;
    [SerializeField] private Image rtanImage;
    [SerializeField] private Image bossImage;

    [SerializeField] private RectTransform bottomUIPanelRectTransform;

    private Vector2 offsetTopPos;
    private Vector2 offsetBottomPos;

    private StageController stageController;

    [SerializeField] private float modifiedDistance = 6.5f;
    private const float HEIGHT_MODIFIED_VALUE = 0.01f;

    private const int BOSS_DEFAULT_INDEX = 20000;

    private Dictionary<int, Sprite> bossSpriteDict = new Dictionary<int, Sprite>();

    private DungeonController dungeonController;

    private Coroutine currentBossCoroutine;

    public void Init()
    {
        gameObject.SetActive(false);
        stageController = FindAnyObjectByType<StageController>();
        float height = Screen.height * HEIGHT_MODIFIED_VALUE;
        offsetTopPos = new Vector2(height, -height);
        offsetBottomPos = new Vector2(-height, height * modifiedDistance);
        stageController.OnBossShowUI += LoadingForBattle;

        dungeonController = FindAnyObjectByType<DungeonController>();
        dungeonController.OnBossShowUI += LoadingForBattle;
    }

    private void UpdateBossSprite()
    {
        int bossIndex = BOSS_DEFAULT_INDEX + stageController.GetCurrentMainStage() - 1;
        GameObject go = ResourceManager.instance.monster.GetResource(bossIndex);
        if (!bossSpriteDict.ContainsKey(bossIndex))
        {
            bossSpriteDict.Add(bossIndex, go.GetComponentInChildren<Animator>().GetComponent<SpriteRenderer>().sprite);
        }

        bossImage.sprite = bossSpriteDict[bossIndex];
    }

    private void UpdateBossSprite(int index)
    {
        GameObject go = ResourceManager.instance.monster.GetResource(index);
        if (!bossSpriteDict.ContainsKey(index))
        {
            bossSpriteDict.Add(index, go.GetComponentInChildren<Animator>().GetComponent<SpriteRenderer>().sprite);
        }

        bossImage.sprite = bossSpriteDict[index];
    }

    private void LoadingForBattle(float loadingDuration)
    {
        StopShowingLoadingPanelCoroutine();
        UpdateBossSprite();
        gameObject.SetActive(true);
        ResetPos();
        currentBossCoroutine = StartCoroutine(CoLoadingForBattle(loadingDuration));
    }

    private void LoadingForBattle(int index, float loadingDuration)
    {
        StopShowingLoadingPanelCoroutine();
        UpdateBossSprite(index);
        gameObject.SetActive(true);
        ResetPos();
        currentBossCoroutine = StartCoroutine(CoLoadingForBattle(loadingDuration));
    }

    public void StopShowingLoadingPanelCoroutine()
    {
        if (currentBossCoroutine != null)
        {
            StopCoroutine(currentBossCoroutine);
            gameObject.SetActive(false);
        }
    }

    private void ResetPos()
    {
        Vector2 topPos = rtanRectTransform.anchoredPosition;
        topPos.x = -offsetTopPos.x;
        topPos.y = offsetTopPos.y;
        rtanRectTransform.anchoredPosition = topPos;
        Vector2 bottomPos = bossRectTransform.anchoredPosition;
        bottomPos.x = offsetTopPos.x;
        bottomPos.y = offsetBottomPos.y;
        bossRectTransform.anchoredPosition = bottomPos;
    }

    private IEnumerator CoLoadingForBattle(float loadingDuration)
    {
        float elapsedTime = Time.deltaTime;
        float ratio = elapsedTime / loadingDuration;

        while (ratio < 1f)
        {
            Vector2 topPos = Vector2.Lerp(rtanRectTransform.anchoredPosition, offsetTopPos, ratio);
            rtanRectTransform.anchoredPosition = topPos;
            Vector2 bottomPos = Vector2.Lerp(bossRectTransform.anchoredPosition, offsetBottomPos, ratio);
            bossRectTransform.anchoredPosition = bottomPos;
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / loadingDuration;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    
}
