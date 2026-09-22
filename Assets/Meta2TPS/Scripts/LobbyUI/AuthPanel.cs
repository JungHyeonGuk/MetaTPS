using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

        await RegisterOrLoadPlayerDataAsync();

        SystemManager.Instance.ShowPanel("HomePanel"); 
    }

    void OnDestroy() 
    {
        if (Authentication.Instance != null)
        {
            _ = Authentication.Instance.SignOutAsync();
        }
    }

    async Awaitable RegisterOrLoadPlayerDataAsync() 
    {
        (bool ok, Exception error, Dictionary<string, object> data) = await CloudSave.Instance.LoadAllPlayerDataAsync(true);

        if (!ok) 
        {
            Debug.LogError(error.Message);
            return;
        }

        if (data.Count == 0) 
        {
            // Register player dictionary
            data = new() 
            {
                { "nickname", $"Player{UnityEngine.Random.Range(1000, 9999)}" },
                { "avatar", "Default"}
            };

            (bool ok2, Exception error2) = await CloudSave.Instance.SavePlayerDataAsync(data, true);

            if (!ok2) 
            {
                Debug.LogError(error2.Message);
                return;
            }
        }

        Model.Instance.playerData = new PlayerData()
        {
            nickname = data["nickname"].ToString(),
            avatar = data["avatar"].ToString()
        };
    }
}
