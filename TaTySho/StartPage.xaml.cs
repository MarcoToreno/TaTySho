namespace TaTySho;

public partial class StartPage : ContentPage
{
    private bool _isAnimating = true;

    public StartPage()
    {
        InitializeComponent();
        // Запускаємо анімацію фону
        AnimateBackground();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isAnimating = true;
        UpdateThemeIcon();

        // Перезапуск анімації, якщо вона зупинилася
        if (Firefly1 != null && !Firefly1.AnimationIsRunning("Fly1"))
            AnimateBackground();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isAnimating = false;
    }

    private async void AnimateBackground()
    {
        // Якщо елементів немає (помилка завантаження), виходимо
        if (Firefly1 == null || Firefly2 == null || Firefly3 == null) return;

        var random = new Random();

        while (_isAnimating)
        {
            await Task.WhenAll(
                Firefly1.TranslateTo(random.Next(-50, 50), random.Next(-50, 50), 4000, Easing.SinInOut),
                Firefly2.TranslateTo(random.Next(-30, 30), random.Next(-30, 30), 5000, Easing.SinInOut),
                Firefly3.TranslateTo(random.Next(-20, 20), random.Next(-60, 60), 6000, Easing.SinInOut),

                Firefly1.FadeTo(random.NextDouble() * 0.5 + 0.2, 4000),
                Firefly2.FadeTo(random.NextDouble() * 0.5 + 0.2, 5000)
            );
        }
    }

    private void UpdateThemeIcon()
    {
        if (ThemeIconLabel != null)
            ThemeIconLabel.Text = GameState.IsDarkTheme ? "☀️" : "🌙";
    }

    private void OnThemeTapped(object sender, EventArgs e)
    {
        GameState.IsDarkTheme = !GameState.IsDarkTheme;
        UpdateThemeIcon();
    }

    private async void OnPlayClicked(object sender, EventArgs e)
    {
        await PlayBtn.ScaleTo(0.95, 100);
        await PlayBtn.ScaleTo(1.0, 100);

        try
        {
            await Shell.Current.GoToAsync(nameof(MenuPage));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}