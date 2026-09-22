using System;
using UnityEngine;
using UnityEngine.Networking;

public class CloudContentDelivery : MonoSingleton<CloudContentDelivery>
{
    [SerializeField] string projectId;
    [SerializeField] string bucketId;



    public async Awaitable<(bool ok, Exception error, string text)> GetTextAsync(string path)
    {
        using var req = UnityWebRequest.Get(ContentUrl(path));
        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) 
        {
            return (false, new Exception(req.error), null);
        }

        return (true, null, req.downloadHandler.text);
    }

    public async Awaitable<(bool ok, Exception error, Texture2D texture)> GetImageAsync(string path)
    {
        using var req = UnityWebRequestTexture.GetTexture(ContentUrl(path));
        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            return (false, new Exception(req.error), null);
        }

        return (true, null, DownloadHandlerTexture.GetContent(req));
    }


    string ContentUrl(string path) 
    {
        return $"https://{projectId}.client-api.unity3dusercontent.com/client_api/v1/environments/production/buckets/{bucketId}/release_by_badge/latest/entry_by_path/content/?path={path}";
    }
}
