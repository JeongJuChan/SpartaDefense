using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideController : MonoBehaviour
{
    [SerializeField] private GuideController guideController;
    void Start()
    {
        guideController.Initialize();
    }

}
