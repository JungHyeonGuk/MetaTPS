using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapLoadingPanel : PanelBase 
{
    [SerializeField] Image thumbImage;
    [SerializeField] Text titleText;
    [SerializeField] Transform loadingImage;
    [SerializeField] float loadingSpeed = 300f;



    public override async void Show()
    {
        base.Show();

        MapData currentMapData = CacheManager.Instance.mapDatas.FirstOrDefault(x => x.id == Model.Instance.currentMapId);
        titleText.text = currentMapData.title;
        thumbImage.sprite = await CacheManager.Instance.LoadMapThumbAsync(currentMapData.id);

        await LoadMapAsync();
    }


    void Update()
    {
        loadingImage.Rotate(0, 0, -loadingSpeed * Time.deltaTime);
    }

    async Awaitable LoadMapAsync() 
    {
        await SceneManager.LoadSceneAsync("PlaygroundScene", LoadSceneMode.Additive);
        await Awaitable.WaitForSecondsAsync(2f);

        LobbyPanelManager.Instance.ShowLobbyObjects(false);
    }
}