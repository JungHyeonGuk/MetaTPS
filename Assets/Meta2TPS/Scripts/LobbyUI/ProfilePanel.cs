using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : PanelBase 
{
    [SerializeField] Button closeBtn;
    [SerializeField] InputField nicknameInput;
    [SerializeField] Image nicknameInputOutline;
    [SerializeField] Button editNicknameBtn;
    [SerializeField] Button editNicknameSaveBtn;
    [SerializeField] Button editNicknameCancelBtn;
    [SerializeField] Button editAvatarBtn;
    [SerializeField] Button signOutBtn;
    [SerializeField] Color outlineRedColor;

    Color outlineOriginColor;




    public override void Init() 
    {
        closeBtn.AddEvent(OnClickCloseBtn);
        editNicknameBtn.AddEvent(OnClickEditNicknameBtn);
        editNicknameSaveBtn.AddEvent(async () => await OnClickEditNicknameSaveBtn());
        editNicknameCancelBtn.AddEvent(OnClickEditNicknameCancelBtn);
        editAvatarBtn.AddEvent(OnClickEditAvatarBtn);
        signOutBtn.AddEvent(async () => await OnClickSignOutBtn());

        outlineOriginColor = nicknameInputOutline.color;
    }

    public override void Show()
    {
        base.Show();
        ActiveUIEditNickname(false);
    }


    void ActiveUIEditNickname(bool active) 
    {
        nicknameInput.gameObject.SetActive(active);
        editNicknameSaveBtn.gameObject.SetActive(active);
        editNicknameCancelBtn.gameObject.SetActive(active);

        editNicknameBtn.gameObject.SetActive(!active);
    }

    void OnClickCloseBtn() 
    {
        LobbyPanelManager.Instance.ShowPanel("HomePanel");
    }

    void OnClickEditNicknameBtn() 
    {
        ActiveUIEditNickname(true);
        
        nicknameInputOutline.color = outlineOriginColor;
        nicknameInput.text = CacheManager.Instance.myPlayerData.nickname;
    }

    async Awaitable OnClickEditNicknameSaveBtn() 
    {
        CacheManager.Instance.myPlayerData.nickname = nicknameInput.text;

        bool ok = await CacheManager.Instance.SaveMyPlayerDataAsync();

        if (!ok) 
        {
            nicknameInputOutline.color = outlineRedColor;
            return;
        }

        ActiveUIEditNickname(false);
    }

    void OnClickEditNicknameCancelBtn() 
    {
        ActiveUIEditNickname(false);
    }

    void OnClickEditAvatarBtn() 
    {
        LobbyPanelManager.Instance.ShowPanel("AvatarPanel");
    }

    async Awaitable OnClickSignOutBtn() 
    {
        await Authentication.Instance.SignOutAsync();

        LobbyPanelManager.Instance.ShowPanel("AuthPanel");
    }
}