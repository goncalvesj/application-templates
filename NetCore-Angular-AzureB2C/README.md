# Intro

Set of application templates that integrate with Azure AD B2C for authentication.

The templates are based on the [ASP.NET Core 2.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) framework.

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

## AD B2C Set Up

TODO

## Getting Started

1. Clone the repository
2. Build the solution
3. Update the `appsettings.json` file with your AD B2C settings
4. Run the API application
5. Run the client applications
