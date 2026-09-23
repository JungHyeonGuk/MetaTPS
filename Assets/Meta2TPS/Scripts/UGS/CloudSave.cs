using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using System;

public class CloudSave : MonoSingleton<CloudSave> 
{
    public async Awaitable<(bool ok, Exception error)> SaveMyPlayerDataAsync(Dictionary<string, object> dic, bool isPublic)
    {
        try
        {
            if (isPublic)
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(dic,
                    new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(
                        new Unity.Services.CloudSave.Models.Data.Player.PublicWriteAccessClassOptions()));
            }
            else
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(dic);
            }

            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    public async Awaitable<(bool ok, Exception error, T value)> LoadMyPlayerDataAsync<T>(string key, bool isPublic)
    {
        try
        {
            Dictionary<string, Unity.Services.CloudSave.Models.Item> data;

            if (isPublic)
            {
                data = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { key },
                    new Unity.Services.CloudSave.Models.Data.Player.LoadOptions(
                        new Unity.Services.CloudSave.Models.Data.Player.PublicReadAccessClassOptions()));
            }
            else
            {
                data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
            }


            if (!data.TryGetValue(key, out var item))
            {
                return (true, null, default);
            }

            return (true, null, item.Value.GetAs<T>());
        }
        catch (Exception e)
        {
            return (false, e, default);
        }
    }

    public async Awaitable<(bool ok, Exception error, Dictionary<string, object> data)> LoadAllMyPlayerDataAsync(bool isPublic)
    {
        try
        {
            Dictionary<string, Unity.Services.CloudSave.Models.Item> data;

            if (isPublic)
            {
                data = await CloudSaveService.Instance.Data.Player.LoadAllAsync(
                    new Unity.Services.CloudSave.Models.Data.Player.LoadAllOptions(
                        new Unity.Services.CloudSave.Models.Data.Player.PublicReadAccessClassOptions()));
            }
            else
            {
                data = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
            }

            Dictionary<string, object> result = new();
            foreach (var pair in data)
            {
                result[pair.Key] = pair.Value.Value.GetAs<object>();
            }

            return (true, null, result);
        }
        catch (Exception e)
        {
            return (false, e, null);
        }
    }
}