using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanel : PanelBase 
{
    [SerializeField] Button closeBtn;
    [SerializeField] GameObject avatarCardPrefab;
    [SerializeField] Transform avatarContent;

    List<AvatarCard> avatarCards = new();
    string oldAvatarId;



    public override void Init() 
    {
        closeBtn.AddEvent(async () => await OnClickCloseBtn());
    }

    public override async void InitShow() 
    {
        await LoadAvatarCardAsync();
    }

    public override void Show()
    {
        base.Show();
        oldAvatarId = CacheManager.Instance.myPlayerData.avatarId;
    }


    async Awaitable OnClickCloseBtn() 
    {
        if (oldAvatarId != CacheManager.Instance.myPlayerData.avatarId) 
        {
            await CacheManager.Instance.SaveMyPlayerDataAsync();
        }

        LobbyPanelManager.Instance.ShowPanel("ProfilePanel");        
    }

    async Awaitable LoadAvatarCardAsync() 
    {
        AvatarData[] avatarDatas = await CacheManager.Instance.LoadAvatarDatasAsync();
        List<Awaitable> loads = new();

        if (avatarDatas == null) 
        {
            Debug.LogError("LoadAvatarDatasAsync failed");
            return;
        }

        foreach (AvatarData avatarData in avatarDatas) 
        {
            AvatarCard avatarCard = Instantiate(avatarCardPrefab, avatarContent).GetComponent<AvatarCard>();
            avatarCard.avatarText.text = avatarData.id;
            avatarCard.btn.AddEvent(() => OnClickAvatarCard(avatarData.id));
            avatarCards.Add(avatarCard);
            loads.Add(LoadAvatarCardSpriteAsync(avatarCard, avatarData.id));

            bool isSelected = avatarData.id == CacheManager.Instance.myPlayerData.avatarId;
            avatarCard.selectedFrame.SetActive(isSelected);
        }
    }

    async Awaitable LoadAvatarCardSpriteAsync(AvatarCard avatarCard, string avatarId) 
    {
        Sprite sprite = await CacheManager.Instance.LoadAvatarAsync(avatarId);
        
        if (sprite != null) 
        {
            avatarCard.avatarImage.sprite = sprite;
        }
    }

    void OnClickAvatarCard(string avatarId) 
    {
        foreach (AvatarCard avatarCard in avatarCards) 
        {
            bool isSelected = avatarCard.avatarText.text == avatarId;
            avatarCard.selectedFrame.SetActive(isSelected);
        }

        CacheManager.Instance.myPlayerData.avatarId = avatarId;
    }
}