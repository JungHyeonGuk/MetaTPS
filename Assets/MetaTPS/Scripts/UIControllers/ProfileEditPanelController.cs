using UnityEngine;

public class ProfileEditPanelController : PanelBase  
{
    [SerializeField] CloseBtnContainer closeBtn;
    [SerializeField] ProfileEditCardContainer displayProfileEditCard;
    [SerializeField] ProfileEditCardContainer usernameProfileEditCard;
    [SerializeField] ProfileEditCardContainer introduceProfileEditCard;



    public override void Init()
    {
        base.Init();
        
        closeBtn.btn.AddEvent(OnClickCloseBtn);
        displayProfileEditCard.btn.AddEvent(OnClickDisplayProfileEditBtn);
        introduceProfileEditCard.btn.AddEvent(OnClickIntroduceProfileEditBtn);
    }
    
    void OnClickCloseBtn()
    {
        PanelManager.Instance.profile.Show();
        Hide();
    }
    
    void OnClickDisplayProfileEditBtn()
    {
        PanelManager.Instance.displaynameEdit.Show();
    }       

    void OnClickIntroduceProfileEditBtn()
    {
        PanelManager.Instance.introduceEdit.Show();
    }
}