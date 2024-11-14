using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CastleHPUIPanel : HPUIPanel
{
    [SerializeField] private TextMeshProUGUI hpText;

    public override void UpdateCurrentHPUI(BigInteger currentHp)
    {
        base.UpdateCurrentHPUI(currentHp);
        UpdateHpText();
    }

    public override void UpdateMaxHP(BigInteger maxHP, BigInteger currentHp)
    {
        base.UpdateMaxHP(maxHP, currentHp);
        UpdateHpText();
    }

    public override void ResetUI()
    {
        base.ResetUI();
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        hpText.text = preHP.ToString();
    }
}
