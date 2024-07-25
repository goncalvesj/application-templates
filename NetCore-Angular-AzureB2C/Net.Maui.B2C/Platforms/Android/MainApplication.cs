// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Android.App;
using Android.Runtime;

namespace Net.Maui.B2C;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
