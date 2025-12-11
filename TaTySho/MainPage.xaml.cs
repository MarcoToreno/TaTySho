using TaTySho.Models;

namespace TaTySho;

public partial class MainPage : ContentPage
{
    List<string> currentRoundQueue;
    int roundScore = 0;
    double timeLeft = 60;
    bool isGameRunning = false;
    IDispatcherTimer timer;
    private bool _isAnimating = true;

    const int SURVIVAL_START_TIME = 20;
    const int BONUS_TIME = 5;
    const int PENALTY_TIME = 5;

    public MainPage()
    {
        InitializeComponent();
        AnimateParticles();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isAnimating = true;

        try
        {
            var team = GameState.GetCurrentTeam();
            string modeName = GameState.GameMode == 0 ? "Класика" : (GameState.GameMode == 1 ? "Виживання" : "1 vs All");
            CurrentTeamLabel.Text = $"{team.Name} | {modeName}";
        }
        catch
        {
            CurrentTeamLabel.Text = "Тест";
        }

        WordLabel.Text = "Готуйся...";
        StartBtn.IsVisible = true;
        GameButtonsGrid.IsVisible = false;
        CardFrame.IsVisible = false;
        CardFrame.Scale = 1; CardFrame.TranslationX = 0; CardFrame.BackgroundColor = Colors.White;

        int start = GameState.GameMode == 1 ? SURVIVAL_START_TIME : GameState.RoundTime;
        TimerLabel.Text = $"00:{start:D2}";
        TimerLabel.TextColor = Colors.White;
    }

    protected override void OnDisappearing() { base.OnDisappearing(); _isAnimating = false; }

    private void OnStartClicked(object sender, EventArgs e) => StartRound();

    private void StartRound()
    {
        StartBtn.IsVisible = false;
        GameButtonsGrid.IsVisible = true;
        CardFrame.IsVisible = true;

        roundScore = 0;
        timeLeft = (GameState.GameMode == 1) ? SURVIVAL_START_TIME : GameState.RoundTime;
        isGameRunning = true;
        UpdateTimerVisuals();

        var sourceDeck = GameState.CurrentDeck ?? new List<string> { "Помилка", "Слів", "Немає" };
        var rnd = new Random();
        currentRoundQueue = sourceDeck.OrderBy(x => rnd.Next()).ToList();

        if (timer != null) timer.Stop();
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        timer.Start();

        ShowNextWord();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        timeLeft--;
        UpdateTimerVisuals();
        if (timeLeft <= 0) EndRound();
    }

    private void UpdateTimerVisuals()
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        TimerLabel.Text = $"{minutes:D2}:{seconds:D2}";

        if (timeLeft <= 5) TimerLabel.TextColor = Colors.Red;
        else if (timeLeft <= 10) TimerLabel.TextColor = Colors.Yellow;
        else if (timeLeft > 60) TimerLabel.TextColor = Colors.LightGreen;
        else TimerLabel.TextColor = Colors.White;
    }

    private async void EndRound()
    {
        isGameRunning = false;
        if (timer != null) timer.Stop();
        if (GameState.Teams.Count > 0) GameState.GetCurrentTeam().TotalScore += roundScore;
        await Shell.Current.GoToAsync(nameof(ResultsPage));
    }

    private void ShowNextWord()
    {
        CardFrame.TranslationX = 0;
        CardFrame.BackgroundColor = Colors.White;
        if (currentRoundQueue != null && currentRoundQueue.Count > 0)
        {
            WordLabel.Text = currentRoundQueue[0];
            currentRoundQueue.RemoveAt(0);
        }
        else EndRound();
    }

    private async Task HandleGuess(bool isCorrect)
    {
        if (!isGameRunning) return;
        var frame = this.FindByName<Frame>("CardFrame");

        if (isCorrect)
        {
            roundScore++;
            await PlaySound("ding.mp3");
            try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); } catch { }

            if (GameState.GameMode == 1)
            {
                timeLeft += BONUS_TIME;
                UpdateTimerVisuals();
                await TimerLabel.ScaleTo(1.2, 100);
                await TimerLabel.ScaleTo(1.0, 100);
            }

            if (frame != null)
            {
                frame.BackgroundColor = Color.FromArgb("#E0F7FA");
                await frame.TranslateTo(500, 0, 150, Easing.CubicIn);
            }
        }
        else
        {
            await PlaySound("wrong.mp3");
            try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); } catch { }

            if (GameState.GameMode == 1)
            {
                timeLeft -= PENALTY_TIME;
                if (timeLeft < 0) timeLeft = 0;
                UpdateTimerVisuals();
                await TimerLabel.ScaleTo(0.9, 100);
                await TimerLabel.ScaleTo(1.0, 100);
            }

            if (frame != null)
            {
                frame.BackgroundColor = Color.FromArgb("#FFEBEE");
                await frame.TranslateTo(-500, 0, 150, Easing.CubicIn);
            }
        }

        if (GameState.GameMode == 1 && timeLeft <= 0) EndRound();
        else ShowNextWord();
    }

    private async void OnSkipClicked(object sender, EventArgs e) => await HandleGuess(false);
    private async void OnGuessClicked(object sender, EventArgs e) => await HandleGuess(true);
    private async void OnSwiped(object sender, SwipedEventArgs e) => await HandleGuess(e.Direction == SwipeDirection.Right);

    private async Task PlaySound(string fileName)
    {
        try
        {
            var player = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(fileName));
            player.Volume = GameState.SoundVolume;
            player.Play();
        }
        catch { }
    }

    private async void AnimateParticles()
    {
        if (Particle1 == null) return;
        var random = new Random();
        var particles = new[] { Particle1, Particle2, Particle3, Particle4, Particle5 };
        while (_isAnimating)
        {
            var tasks = new List<Task>();
            foreach (var particle in particles)
            {
                tasks.Add(particle.TranslateTo(random.Next(-30, 30), random.Next(-30, 30), (uint)random.Next(4000, 8000), Easing.SinInOut));
                tasks.Add(particle.FadeTo(random.NextDouble() * 0.3 + 0.1, (uint)random.Next(4000, 8000)));
            }
            await Task.WhenAll(tasks);
        }
    }
}