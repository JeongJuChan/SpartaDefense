using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeInfoPanel : MonoBehaviour
{
    [SerializeField] private GuideController guideController;
    [SerializeField] private Button forgeInfoButton;

    private QuestManager questManager;

    public void Init()
    {
        questManager = QuestManager.instance;
        
    }

    void Start()
    {
        guideController.Initialize();
    }
}
