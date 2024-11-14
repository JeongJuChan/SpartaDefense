using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLevelController : MonoBehaviour
{
    public UserLevelDataHandler userLevelDataHandler { get; private set; }

    [SerializeField] private UserLevelDataSO userLevelDataSO;

    private UI_Castle uiCastle;

    public void Init()
    {
        UserLevelData userLevelData = userLevelDataSO.GetData();
        userLevelDataHandler = new UserLevelDataHandler(userLevelData);
        userLevelDataHandler.OnUpdateCastleQuest += UpdateCastleQuest;
    }
    
    public void StartInit()
    {
        userLevelDataHandler.StartInit();
    }

    public void InitLevelUI()
    {
        userLevelDataHandler.UpdateExp(0);
    }

    private void UpdateCastleQuest()
    {
        if (uiCastle == null)
        {
            uiCastle = UIManager.instance.GetUIElement<UI_Castle>();
        }

        uiCastle.UpdateCastleQuestUI();
    }
}
