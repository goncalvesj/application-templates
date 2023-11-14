// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Foundation;
using Net.Maui.B2C.MSALClient;
using Microsoft.Identity.Client;
using UIKit;
namespace Net.Maui.B2C;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Initialize MSAL and platformConfig is set
        IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;

        return base.FinishedLaunching(application, launchOptions);
    }
}
