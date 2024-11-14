using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityOptionGenerator
{
    private AbilityOptionGradeSelector abilityOptionGradeSelector;
    private AbilityOptionEffectSelector abilityOptionEffectSelector;
    private AbilityOptionCalculator abilityOptionCalculator;

    public AbilityOptionGenerator(AbilityOptionGradeSelector abilityOptionGradeSelector, AbilityOptionEffectSelector abilityOptionEffectSelector, AbilityOptionCalculator abilityOptionCalculator)
    {
        this.abilityOptionGradeSelector = abilityOptionGradeSelector;
        this.abilityOptionEffectSelector = abilityOptionEffectSelector;
        this.abilityOptionCalculator = abilityOptionCalculator;
    }

    public void GenerateAbilityOption(out Rank rank, out AbilityOptionEffectType effectType, out float multiplier, out ArithmeticStatType arithmeticStatType)
    {
        rank = abilityOptionGradeSelector.SelectGrade();
        effectType = abilityOptionEffectSelector.SelectEffect();
        multiplier = abilityOptionCalculator.DetermineEffectIncrease(effectType, rank);
        arithmeticStatType = abilityOptionCalculator.DetermineStatType(effectType, rank);
    }
}
