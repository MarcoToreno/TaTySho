namespace TaTySho;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // Завантажуємо налаштування
        ScoreSlider.Value = GameState.TargetScore;
        ScoreLabel.Text = $"{GameState.TargetScore} очок";

        // НОВЕ: Час
        TimeSlider.Value = GameState.RoundTime;
        TimeLabel.Text = $"{GameState.RoundTime} сек";

        VolumeSlider.Value = GameState.SoundVolume;
        ThemeSwitch.IsToggled = GameState.IsDarkTheme;
    }

    private void OnScoreChanged(object sender, ValueChangedEventArgs e)
    {
        double value = Math.Round(e.NewValue / 5.0) * 5;
        ScoreLabel.Text = $"{value} очок";
    }

    // НОВЕ: Обробка зміни часу (крок 10 сек)
    private void OnTimeChanged(object sender, ValueChangedEventArgs e)
    {
        double value = Math.Round(e.NewValue / 10.0) * 10; // Крок по 10 секунд
        if (value < 10) value = 10; // Мінімум 10 сек
        TimeLabel.Text = $"{value} сек";
    }

    private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        GameState.SoundVolume = e.NewValue;
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        GameState.IsDarkTheme = e.Value;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Зберігаємо Очки
        double score = Math.Round(ScoreSlider.Value / 5.0) * 5;
        GameState.TargetScore = (int)score;

        // Зберігаємо Час
        double time = Math.Round(TimeSlider.Value / 10.0) * 10;
        if (time < 10) time = 10;
        GameState.RoundTime = (int)time;

        await Shell.Current.GoToAsync("..");
    }
}