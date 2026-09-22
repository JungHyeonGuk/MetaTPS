using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using System.Threading.Tasks;


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

    public async Awaitable<(bool ok, Exception error)> SignInWithUnityAsync()
    {
        (bool ok, Exception error) = await EnsureInitializedAsync();
        if (!ok) return (false, error);
        
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
                return (true, null);
            if (!PlayerAccountService.Instance.IsSignedIn)
            {
                var tcs = new TaskCompletionSource<bool>();
                void OnSignedIn()
                {
                    PlayerAccountService.Instance.SignedIn -= OnSignedIn;
                    PlayerAccountService.Instance.SignInFailed -= OnFailed;
                    tcs.TrySetResult(true);
                }
                void OnFailed(RequestFailedException ex)
                {
                    PlayerAccountService.Instance.SignedIn -= OnSignedIn;
                    PlayerAccountService.Instance.SignInFailed -= OnFailed;
                    tcs.TrySetException(ex);
                }
                PlayerAccountService.Instance.SignedIn += OnSignedIn;
                PlayerAccountService.Instance.SignInFailed += OnFailed;

                await PlayerAccountService.Instance.StartSignInAsync();
                await tcs.Task;
            }
            await AuthenticationService.Instance.SignInWithUnityAsync(
                PlayerAccountService.Instance.AccessToken);

            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }

    public async Awaitable<(bool ok, Exception error)> SignOutAsync()
    {
        (bool ok, Exception error) = await EnsureInitializedAsync();
        if (!ok) return (false, error);

        try
        {
            if (!AuthenticationService.Instance.IsSignedIn) 
            {
                return (true, null);
            }
            AuthenticationService.Instance.SignOut(true);
            PlayerAccountService.Instance.SignOut();
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }
}