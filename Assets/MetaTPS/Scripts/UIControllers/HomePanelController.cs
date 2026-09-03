using UnityEngine;
using TMPro;

public class HomePanelController : PanelBase  
{
    [SerializeField] InputContainer searchInput;
    [SerializeField] ProfileBtnContainer profileBtn;
    [SerializeField] TMP_Text headerText;
    [SerializeField] Transform gameContent;
    [SerializeField] GameObject homeGameCardPrefab;



    public override void Init()
    {
        base.Init();
        searchInput.input.AddSubmitEvent(OnSearchInputSubmit);
        profileBtn.btn.AddEvent(OnClickProfileBtn);
        SpawnGameCards();
    }


    void OnSearchInputSubmit(string value) 
    {
        Debug.Log("OnSearchInputSubmit: " + value);
    }
    
    void OnClickProfileBtn() 
    {
        PanelManager.Instance.profile.Show();
        Hide();
    }

    void SpawnGameCards() 
    {
        int count = 10;
        for (int i = 0; i < count; i++) 
        {
            Debug.Log("SpawnGameCards: " + i);
            GameObject gameCard = Instantiate(homeGameCardPrefab, gameContent);
        }
    }

}