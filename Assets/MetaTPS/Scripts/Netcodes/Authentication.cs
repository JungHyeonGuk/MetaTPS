using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;


public class Authentication : MonoSingleton<Authentication> 
{
    bool isInitialized;


    
    public async Awaitable<(bool ok, Exception error)> EnsureInitializedAsync()
    {
        if (isInitialized) return (true, null);

        try
        {
            await UnityServices.InitializeAsync();
            isInitialized = true;
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    public async Awaitable<(bool ok, Exception error)> SignUpUsernameAsync(string username, string password) 
    {
        (bool ok, Exception error) = await EnsureInitializedAsync();
        if (!ok) return (false, error);

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    public async Awaitable<(bool ok, Exception error)> SignInUsernameAsync(string username, string password) 
    {
        (bool ok, Exception error) = await EnsureInitializedAsync();
        if (!ok) return (false, error);

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }
}