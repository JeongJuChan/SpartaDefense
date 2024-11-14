using System;
using UnityEngine;
using UnityEngine.UI;

public class ColleagueIdlePanel : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image colleagueAreaImage;

    private Vector2 rtanPivot;
    private Vector2 colleaguePivot;

    private const float colleaguePivotY = 0.3f;

    private int[] animationClipsHash;

    private Vector2 offsetPos;
    [SerializeField] private Vector2 colleaguePos = new Vector2(2.5f, 25f);


    public void Init()
    {
        offsetPos = rectTransform.anchoredPosition;

        string[] colleagueTypes = Enum.GetNames(typeof(ColleagueType));

        AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
        animationClipsHash = new int[animationClips.Length];
        foreach (var clip in animationClips)
        {
            int index = -1;
            for (int i = 4; i < colleagueTypes.Length; i++)
            {
                if (clip.name.Contains(colleagueTypes[i].Split('_')[0].ToString()))
                {
                    index = i - 4;
                    string fullName = $"{AnimatorParameters.DEFAULT_LAYER_NAME}{clip.name}";
                    animationClipsHash[index] = Animator.StringToHash(fullName);
                }
            }
        }
        
        rtanPivot = rectTransform.pivot;
        colleaguePivot = rtanPivot;
        colleaguePivot.y = colleaguePivotY;
    }

    public void UpdateIndex(ColleagueType colleagueType, Rank rank)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (colleagueType == ColleagueType.Rtan_Rare)
        {
            UpdatePivotPos(colleagueType, rtanPivot);
            ChangeColleagueAreaColor(rank);
        }
        else
        {
            UpdatePivotPos(colleagueType, colleaguePivot);
            ChangeColleagueAreaColor(rank);
        }

        animator.Play(animationClipsHash[(int)colleagueType -4]);
    }

    private void ChangeColleagueAreaColor(Rank rank)
    {
        colleagueAreaImage.color = ResourceManager.instance.rank.GetRankColor(rank);
    }

    private void UpdatePivotPos(ColleagueType colleagueType, Vector2 pivot)
    {
        rectTransform.pivot = pivot;
        Vector2 pos = colleagueType == ColleagueType.Rtan_Rare ? offsetPos : colleaguePos;
        rectTransform.anchoredPosition = pos;
    }
}
