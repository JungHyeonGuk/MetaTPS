using UnityEngine;

public class AvatarEditPanelController : PanelBase  
{
    [SerializeField] CloseBtnContainer closeBtn;


    public override void Init()
    {
        base.Init();
        
        closeBtn.btn.AddEvent(OnClickCloseBtn);
    }

    void OnClickCloseBtn()
    {
        PanelManager.Instance.profile.Show();
        Hide();
    }
}