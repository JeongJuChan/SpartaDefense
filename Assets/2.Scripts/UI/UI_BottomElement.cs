using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BottomElement : UI_Popup
{
    public Button cloaseBtn;
    public Action openUI;

    public override void Initialize()
    {
        base.Initialize();
        InitCamera();
    }

    public virtual void StartInit()
    {

    }
}
