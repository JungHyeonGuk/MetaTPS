using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapDetailPanel : PanelBase 
{
    [SerializeField] Button closeBtn;
    [SerializeField] Image thumbImage;
    [SerializeField] Text titleText;
    [SerializeField] Button playBtn;



    public override void Init() 
    {
        closeBtn.AddEvent(OnClickCloseBtn);
        playBtn.AddEvent(OnClickPlayBtn);
    }

    public override async void Show()
    {
        base.Show();

        MapData currentMapData = CacheManager.Instance.mapDatas.FirstOrDefault(x => x.id == Model.Instance.currentMapId);
        titleText.text = currentMapData.title;
        thumbImage.sprite = await CacheManager.Instance.LoadMapThumbAsync(currentMapData.id);
    }


    void OnClickCloseBtn() 
    {
        LobbyPanelManager.Instance.ShowPanel("HomePanel");
    }

    void OnClickPlayBtn() 
    {
        LobbyPanelManager.Instance.ShowPanel("MapLoadingPanel");
    }
}