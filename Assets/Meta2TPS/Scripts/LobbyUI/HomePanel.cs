using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HomePanel : PanelBase 
{
    [SerializeField] Button profileBtn;
    [SerializeField] GameObject homeCardPrefab;
    [SerializeField] Transform homeContent;

    List<HomeCard> homeCards = new();



    public override void Init() 
    {
        profileBtn.AddEvent(OnClickProfileBtn);
    }

    public override async void InitShow() 
    {
        await LoadHomeCardAsync();
    }


    void OnClickProfileBtn() 
    {
        LobbyPanelManager.Instance.ShowPanel("ProfilePanel");
    }

    async Awaitable LoadHomeCardAsync() 
    {
        MapData[] mapDatas = await CacheManager.Instance.LoadMapDatasAsync();
        List<Awaitable> loads = new();

        if (mapDatas == null) 
        {
            Debug.LogError("LoadMapDatasAsync failed");
            return;
        }

        foreach (MapData mapData in mapDatas) 
        {
            HomeCard homeCard = Instantiate(homeCardPrefab, homeContent).GetComponent<HomeCard>();
            homeCard.titleText.text = mapData.title;
            homeCard.btn.AddEvent(() => OnClickHomeCard(mapData.id));
            homeCards.Add(homeCard);
            loads.Add(LoadHomeCardThumbAsync(homeCard, mapData.id));
        }
    }

    async Awaitable LoadHomeCardThumbAsync(HomeCard homeCard, string mapId) 
    {
        Sprite sprite = await CacheManager.Instance.LoadMapThumbAsync(mapId);
        
        if (sprite != null) 
        {
            homeCard.thumbImage.sprite = sprite;
        }
    }

    void OnClickHomeCard(string mapId) 
    {
        Model.Instance.currentMapId = mapId;
        LobbyPanelManager.Instance.ShowPanel("MapDetailPanel");
    }
}