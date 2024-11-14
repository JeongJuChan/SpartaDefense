using UnityEngine;

[CreateAssetMenu(menuName = "SO/RepeatQuestUpgradeValueData")]
public class RepeatQuestUpgradeValueDataSO : ScriptableObject
{
    [SerializeField] private int[] repeatQuestUpgradeValues;

    public int[] RepeatQuestUpgradeValues => repeatQuestUpgradeValues;

    public void SetRepeatQuestUpgradeValues(int[] values)
    {
        repeatQuestUpgradeValues = values;
    }
}