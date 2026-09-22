using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

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
        await LoadHomeCardCCDAsync();
    }


    void OnClickProfileBtn() 
    {
        Debug.Log("OnClickProfileBtn");
    }

    async Awaitable LoadHomeCardCCDAsync() 
    {
        (bool ok, Exception error, string homeCardJson) = await CloudContentDelivery.Instance.GetTextAsync("MapCard.json");
        if (!ok) 
        {
            Debug.LogError(error.Message);
            return;
        }

        JObject homeCardJObject = JObject.Parse(homeCardJson);
        List<JToken> homeCardJTokens = homeCardJObject["mapCards"].ToList();
        List<Awaitable> loads = new();

        foreach (JToken homeCardJToken in homeCardJTokens) 
        {
            string title = homeCardJToken["title"].ToString();
            string id = homeCardJToken["id"].ToString();
            string imagePath = $"MapThumb/{id}.jpg";

            HomeCard homeCard = Instantiate(homeCardPrefab, homeContent).GetComponent<HomeCard>();
            homeCard.titleText.text = title;
            homeCards.Add(homeCard);
            loads.Add(LoadHomeCardSpriteAsync(homeCard, imagePath));
        }
    }

    async Awaitable LoadHomeCardSpriteAsync(HomeCard homeCard, string imagePath) 
    {
        string imageFilePath = Path.Combine(Application.persistentDataPath, imagePath);
        Texture2D imageTexture;

        if (File.Exists(imageFilePath)) 
        {
            imageTexture = new Texture2D(2, 2);
            imageTexture.LoadImage(File.ReadAllBytes(imageFilePath));
        }
        else 
        {
            (bool ok, Exception error, Texture2D texture) = await CloudContentDelivery.Instance.GetImageAsync(imagePath);
            if (!ok) 
            {
                Debug.LogError(error.Message);
                return;
            }
            imageTexture = texture;

            string imageDirectoryPath = Path.GetDirectoryName(imageFilePath);
            if (!Directory.Exists(imageDirectoryPath))
            {
                Directory.CreateDirectory(imageDirectoryPath);
            }

            File.WriteAllBytes(imageFilePath, imageTexture.EncodeToJPG());
        }

        Sprite imageSprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
        homeCard.thumbImage.sprite = imageSprite;
    }
}