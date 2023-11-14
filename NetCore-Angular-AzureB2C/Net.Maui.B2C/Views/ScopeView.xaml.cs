using Net.Maui.B2C.MSALClient;
using Microsoft.Identity.Client;

namespace Net.Maui.B2C.Views;

public partial class ScopeView : ContentPage
{
    public IEnumerable<string> AccessTokenScopes { get; set; } = new string[] {"No scopes found in access token"};
    public ScopeView()
    {
        BindingContext = this;
        InitializeComponent();

        _ = SetViewDataAsync();
    }

    private async Task SetViewDataAsync()
    {
        try
        {
            _ = await PublicClientSingleton.Instance.AcquireTokenSilentAsync();

            ExpiresAt.Text = PublicClientSingleton.Instance.MSALClientHelper.AuthResult.ExpiresOn.ToLocalTime().ToString();
            AccessTokenScopes = PublicClientSingleton.Instance.MSALClientHelper.AuthResult.Scopes
                .Select(s => s.Split("/").Last());

            Scopes.ItemsSource = AccessTokenScopes;
        }

        catch (MsalUiRequiredException)
        {
            await Shell.Current.GoToAsync("scopeview");
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PublicClientSingleton.Instance.SignOutAsync().ContinueWith((t) =>
        {
            return Task.CompletedTask;
        });

        await Shell.Current.GoToAsync("mainview");
    }

    private async void OnMoveToWebClicked(object sender, EventArgs e)
    {
        // Redirect to web browser
        // TODO: Get this URL from the appsettings.json file
        await Launcher.OpenAsync(new Uri("https://localhost:7006/claims"));
    }
}