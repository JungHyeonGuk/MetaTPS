using System;
using UnityEngine;
using UnityEngine.UI;

public class AuthPanel : PanelBase
{
    [SerializeField] Button continueWithUnityBtn;



    public override void Init()
    {
        continueWithUnityBtn.AddEvent(async () => await OnClickContinueWithUnityBtn());
    }


    async Awaitable OnClickContinueWithUnityBtn() 
    {
        (bool ok, Exception error) = await Authentication.Instance.SignInWithUnityAsync();
        if (!ok) 
        {
            Debug.LogError(error.Message);
            return;
        }

        PlayerData playerData = await CacheManager.Instance.LoadOrRegisterMyPlayerDataAsync();
        if (playerData == null) 
        {
            Debug.LogError("Failed to load player data");
            return;
        }

        LobbyPanelManager.Instance.ShowPanel("HomePanel"); 
    }

    void OnDestroy() 
    {
        if (Authentication.Instance != null)
        {
            _ = Authentication.Instance.SignOutAsync();
        }
    }
}
