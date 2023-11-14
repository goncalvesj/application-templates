# Intro

Set of application templates that integrate with Azure AD B2C for authentication using MSAL.NET.

There are 4 application templates:

- `Net.API.ADB2C` - .Net 7 Web API
- `Net.Blazor.ADB2C` - .Net 7 Blazor WebAssembly Client, uses Authorization Code Flow with PKCE
- `Net.MAUI.ADB2C` - .Net 6 MAUI Client
- `Net.MVC.ADB2C` - .Net 7 MVC Client, uses Authorization Code Flow with PKCE and requires a client secret for downstream APIs

The purpose is to have a set of templates that can be used to quickly get started with Azure AD B2C authentication and test SSO scenarios as well as calling downstream APIs.

## Application Templates

`Net.API.ADB2C`

Example `appsettings.json`:

```json
"AzureAdB2C": {
  "Instance": "https://CHANGE_ME.b2clogin.com",
  "Domain": "CHANGE_ME.onmicrosoft.com",
  "ClientId": "CHANGE_ME", //Change this to your client id
  "CallbackPath": "/signin-oidc",
  "SignedOutCallbackPath": "/signout/B2C_1_SignUpSignIn", //Change this to your sign-up/sign-in policy name
  "SignUpSignInPolicyId": "B2C_1_SignUpSignIn" //Change this to your sign-up/sign-in policy name
}
```

`Net.Blazor.ADB2C`

Example `appsettings.json`:

```json
"AzureAdB2C": {
  "Authority": "https://CHANGE_ME.b2clogin.com/CHANGE_ME.onmicrosoft.com/B2C_1_SignUpSignIn",
  "ClientId": "CHANGE_ME", //Change this to your client id
  "ValidateAuthority": false
},
"WeatherApi": {
  "BaseAddress": "https://localhost:7093",
  "Scopes": [
    "https://CHANGE_ME.onmicrosoft.com/CHANGE_ME/weather.read"
  ]
}
```

`Net.MVC.ADB2C`

Example `appsettings.json`:

```json
  "AzureAdB2C": {
    "Instance": "https://CHANGE_ME.b2clogin.com",
    "Domain": "CHANGE_ME.onmicrosoft.com",
    "TenantId": "CHANGE_ME", //Change this to your tenant id
    "ClientId": "CHANGE_ME", //Change this to your client id
    "ClientSecret": "CHANGE_ME", //Only required for web apps that call web APIs
    "SignUpSignInPolicyId": "B2C_1_SignUpSignIn", //Change this to your sign-up/sign-in policy name
    "CallbackPath": "/signin-oidc",
    "ResponseType": "code",
    "UsePkce": true,
    "Scope": [ "openid", "offline_access" ]
  },
  "WeatherApi": {
    "BaseAddress": "https://localhost:7093",
    "Scope": "https://CHANGE_ME.onmicrosoft.com/CHANGE_ME/weather.read"    
  },
```

`Net.MAUI.ADB2C`

Example `appsettings.json`:

```json
  {
  "AzureAdB2C": {
    "Instance": "https://CHANGE_ME.b2clogin.com",
    "Domain": "CHANGE_ME.onmicrosoft.com",
    "TenantId": "CHANGE_ME",
    "ClientId": "CHANGE_ME",
    "SignUpSignInPolicyId": "B2C_1_SignUpSignIn",
    "ResetPasswordPolicyId": "b2c_1_reset",
    "EditProfilePolicyId": "b2c_1_edit_profile",
    "CacheFileName": "netcore_winui_cache.txt",
    "CacheDir": "C:/temp"
  },
  "DownstreamApi": {
    "Scopes": "https://CHANGE_ME.onmicrosoft.com/CHANGE_ME/weather.read"
  },
  "iOSKeyChainGroup":  "com.microsoft.adalcache"
}
```

For Android In `MsalActivity.cs` replace:
  
```csharp
  [Activity(Exported =true)]
  [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "auth",
      DataScheme = "msal[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]")]
```

## AD B2C Set Up

TODO

## Getting Started

1. Clone the repository
2. Build the solution
3. Update the `appsettings.json` files with your AD B2C settings for the several apps
4. Run the API application
5. Run the client applications
