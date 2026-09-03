using UnityEngine.UI;
using UnityEngine;

public class ProfilePanelController : PanelBase  
{
    [SerializeField] Image avatarImage;
    [SerializeField] ProfileBtnContainer profileBtn;
    [SerializeField] BtnContainer editAvatarBtn;
    [SerializeField] BtnContainer editProfileBtn;
    [SerializeField] BtnContainer signOutBtn;
    [SerializeField] CloseBtnContainer closeBtn;



    public override void Init()
    {
        base.Init();
        
        editAvatarBtn.btn.AddEvent(OnClickEditAvatarBtn);
        editProfileBtn.btn.AddEvent(OnClickEditProfileBtn);
        signOutBtn.btn.AddEvent(async () => await OnClickSignOutBtn());
        closeBtn.btn.AddEvent(OnClickCloseBtn);
    }

 
    void OnClickEditAvatarBtn() 
    {
        PanelManager.Instance.avatarEdit.Show();
        Hide();
    }

    void OnClickEditProfileBtn()
    {
        PanelManager.Instance.profileEdit.Show();
        Hide();
    }

    async Awaitable OnClickSignOutBtn()
    {
        await Authentication.Instance.SignOutAsync();
        PanelManager.Instance.auth.Show();
        Hide();
    }

    void OnClickCloseBtn() 
    {
        PanelManager.Instance.home.Show();
        Hide();
    }
}