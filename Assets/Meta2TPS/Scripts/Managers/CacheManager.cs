using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using UnityEngine.Events;

public class CacheManager : MonoSingleton<CacheManager> 
{
    public UnityAction<PlayerData> myPlayerDataChanged;
    public PlayerData myPlayerData;
    public MapData[] mapDatas;
    public AvatarData[] avatarDatas;

    Dictionary<string, Sprite> mapThumbDic = new(); // mapId
    Dictionary<string, Sprite> avatarDic = new(); // avatarId
    Dictionary<string, Sprite> avatarSkinDic = new(); // avatarId



#region MyPlayerData

    public async Awaitable<PlayerData> LoadOrRegisterMyPlayerDataAsync() 
    {
        (bool ok, Exception error, Dictionary<string, object> data) = await CloudSave.Instance.LoadAllMyPlayerDataAsync(true);

        if (!ok) 
        {
            Debug.LogError(error.Message);
            return null;
        }

        if (data.Count == 0) 
        {
            RegisterMyPlayerData();
            data = GetMyPlayerData();

            (bool ok2, Exception error2) = await CloudSave.Instance.SaveMyPlayerDataAsync(data, true);

            if (!ok2) 
            {
                Debug.LogError(error2.Message);
                return null;
            }
        }

        SetMyPlayerData(data);

        return myPlayerData;
    }

    public async Awaitable<bool> SaveMyPlayerDataAsync() 
    {
        (bool ok, Exception error) = await CloudSave.Instance.SaveMyPlayerDataAsync(GetMyPlayerData(), true);

        if (!ok) 
        {
            Debug.LogError(error.Message);
            return false;
        }

        myPlayerDataChanged?.Invoke(myPlayerData);

        return true;
    }


    void RegisterMyPlayerData() 
    {
        myPlayerData = new()
        {
            nickname = $"Player{UnityEngine.Random.Range(1000, 9999)}",
            avatarId = "Default"
        };
    }

    void SetMyPlayerData(Dictionary<string, object> data) 
    {
        myPlayerData = new()
        {
            nickname = data["nickname"].ToString(),
            avatarId = data["avatarId"].ToString()
        };
        myPlayerDataChanged?.Invoke(myPlayerData);
    }

    Dictionary<string, object> GetMyPlayerData() 
    {
        return new() 
        {
            { "nickname", myPlayerData.nickname },
            { "avatarId", myPlayerData.avatarId }
        };
    }


#endregion


#region MapData 

    public async Awaitable<MapData[]> LoadMapDatasAsync() 
    {
        if (mapDatas != null && mapDatas.Length > 0) 
        {
            return mapDatas;
        }

        (bool ok, Exception error, string data) = await CloudContentDelivery.Instance.GetTextAsync("MapCard.json");

        if (!ok) 
        {
            Debug.LogError(error.Message);
            return null;
        }

        List<JToken> jTokens = JObject.Parse(data)["mapCards"].ToList();
        mapDatas = new MapData[jTokens.Count];

        for (int i = 0; i < jTokens.Count; i++) 
        {
            mapDatas[i] = new MapData()
            {
                title = jTokens[i]["title"].ToString(),
                id = jTokens[i]["id"].ToString()
            };
        }

        return mapDatas;
    }

    public async Awaitable<Sprite> LoadMapThumbAsync(string mapId) 
    {
        if (mapThumbDic.TryGetValue(mapId, out Sprite sprite)) 
        {
            return sprite;
        }

        Sprite sprite2 = await LoadLocalOrRemoteSpriteAsync($"MapThumb/{mapId}.jpg");

        if (sprite2 != null) 
        {
            mapThumbDic[mapId] = sprite2;
        }

        return sprite2;
    } 

#endregion
   

#region Avatar 

    public async Awaitable<AvatarData[]> LoadAvatarDatasAsync() 
    {
        if (avatarDatas != null && avatarDatas.Length > 0) 
        {
            return avatarDatas;
        }

        (bool ok, Exception error, string data) = await CloudContentDelivery.Instance.GetTextAsync("AvatarCard.json");

        if (!ok) 
        {
            Debug.LogError(error.Message);
            return null;
        }

        List<JToken> jTokens = JObject.Parse(data)["avatarCards"].ToList();
        avatarDatas = new AvatarData[jTokens.Count];

        for (int i = 0; i < jTokens.Count; i++) 
        {
            avatarDatas[i] = new AvatarData()
            {
                id = jTokens[i]["id"].ToString()
            };
        }

        return avatarDatas;
    }

    public async Awaitable<Sprite> LoadAvatarAsync(string avatarId) 
    {
        if (avatarDic.TryGetValue(avatarId, out Sprite sprite)) 
        {
            return sprite;
        }

        Sprite sprite2 = await LoadLocalOrRemoteSpriteAsync($"Avatar/{avatarId}.jpg");

        if (sprite2 != null) 
        {
            avatarDic[avatarId] = sprite2;
        }

        return sprite2;
    }

    public async Awaitable<Sprite> LoadAvatarSkinAsync(string avatarId) 
    {
        if (avatarSkinDic.TryGetValue(avatarId, out Sprite sprite)) 
        {
            return sprite;
        }

        Sprite sprite2 = await LoadLocalOrRemoteSpriteAsync($"AvatarSkin/{avatarId}.png");

        if (sprite2 != null) 
        {
            avatarSkinDic[avatarId] = sprite2;
        }

        return sprite2;
    }

#endregion


#region Common

    public async Awaitable<Sprite> LoadLocalOrRemoteSpriteAsync(string remoteImagePath) 
    {
        string localImagePath = Path.Combine(Application.persistentDataPath, remoteImagePath);
        Texture2D imageTexture;

        if (File.Exists(localImagePath)) 
        {
            imageTexture = new Texture2D(2, 2);
            imageTexture.LoadImage(File.ReadAllBytes(localImagePath));
        }
        else 
        {
            if (!Directory.Exists(Path.GetDirectoryName(localImagePath))) 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localImagePath));
            }

            (bool ok, Exception error, Texture2D texture) = await CloudContentDelivery.Instance.GetImageAsync(remoteImagePath);
            if (!ok) 
            {
                Debug.LogError(error.Message);
                return null;
            }

            imageTexture = texture;

            string extension = Path.GetExtension(remoteImagePath).ToLowerInvariant();

            if (extension == ".jpg" || extension == ".jpeg") 
            {
                File.WriteAllBytes(localImagePath, imageTexture.EncodeToJPG());
            }
            else if (extension == ".png") 
            {
                File.WriteAllBytes(localImagePath, imageTexture.EncodeToPNG());
            }
            else 
            {
                Debug.LogError($"Unsupported image format: {extension}");
                return null;
            }
        }

        Sprite imageSprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
        return imageSprite;
    }


#endregion
}