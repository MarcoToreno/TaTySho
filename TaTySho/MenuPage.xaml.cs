using System.Collections.ObjectModel;
using TaTySho.Models;
using System.Text.Json;

namespace TaTySho;

public partial class MenuPage : ContentPage
{
    public ObservableCollection<Team> LobbyTeams { get; set; } = new ObservableCollection<Team>();
    private bool _isAnimating = true;

    public MenuPage()
    {
        InitializeComponent();

        // Початкові команди
        LobbyTeams.Add(new Team { Name = "Варенички" });
        LobbyTeams.Add(new Team { Name = "Козаки" });

        // Прив'язка списку до інтерфейсу
        TeamsList.ItemsSource = LobbyTeams;

        LoadDecks();
        AnimateParticles();
    }

    // --- МЕТОДИ ДЛЯ КНОПОК ---

    // 1. Додати команду
    private void OnAddTeamClicked(object sender, EventArgs e)
    {
        LobbyTeams.Add(new Team { Name = "" });
    }

    // 2. Видалити команду (ОСЬ ЦЕЙ МЕТОД, ЯКОГО НЕ ВИСТАЧАЛО)
    private void OnRemoveTeamClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var teamToDelete = button.CommandParameter as Team;

        if (LobbyTeams.Count > 2)
        {
            LobbyTeams.Remove(teamToDelete);
        }
        else
        {
            DisplayAlert("Упс", "Для гри потрібно мінімум 2 команди", "Ок");
        }
    }

    // 3. Зміна режиму гри
    private void OnModeChanged(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (int.TryParse(btn.CommandParameter.ToString(), out int mode))
        {
            GameState.GameMode = mode;
            UpdateModeButtonsUI(mode);
        }
    }

    // 4. Перехід у налаштування
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    // 5. Натискання на колоду (Старт гри)
    private async void OnDeckTapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        var selectedDeck = e.Parameter as Deck;

        if (selectedDeck == null) return;

        // Анімація натискання
        if (frame != null)
        {
            await frame.ScaleTo(0.95, 100);
            await frame.ScaleTo(1.0, 100);
        }

        // Вибір складності
        string difficulty = await DisplayActionSheet(
            $"Рівень для '{selectedDeck.Name}':",
            "Скасувати",
            null,
            "🟢 Легко",
            "🟡 Середньо",
            "🔴 Важко");

        if (difficulty == "Скасувати" || difficulty == null) return;

        // Підбір слів
        List<string> wordsToPlay = new List<string>();
        if (difficulty == "🟢 Легко") wordsToPlay = selectedDeck.EasyWords;
        else if (difficulty == "🟡 Середньо") wordsToPlay = selectedDeck.MediumWords;
        else if (difficulty == "🔴 Важко") wordsToPlay = selectedDeck.HardWords;

        // Якщо список пустий - беремо загальний або заглушку
        if (wordsToPlay == null || wordsToPlay.Count == 0)
            wordsToPlay = selectedDeck.Words ?? new List<string> { "Слів немає" };

        // Перевірка імен команд
        int i = 1;
        foreach (var team in LobbyTeams)
        {
            if (string.IsNullOrWhiteSpace(team.Name)) team.Name = $"Команда {i}";
            i++;
        }

        // ЗАПУСК ГРИ
        GameState.StartNewGame(LobbyTeams.ToList(), wordsToPlay);
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    // --- ДОПОМІЖНІ МЕТОДИ ---

    private void UpdateModeButtonsUI(int mode)
    {
        // Скидаємо кольори
        BtnModeClassic.BackgroundColor = Colors.Transparent;
        BtnModeClassic.TextColor = Colors.White;

        BtnModeSurvival.BackgroundColor = Colors.Transparent;
        BtnModeSurvival.TextColor = Colors.White;

        BtnModeSolo.BackgroundColor = Colors.Transparent;
        BtnModeSolo.TextColor = Colors.White;

        // Підсвічуємо активну кнопку
        Button activeBtn = mode switch
        {
            0 => BtnModeClassic,
            1 => BtnModeSurvival,
            _ => BtnModeSolo
        };

        if (activeBtn != null)
        {
            activeBtn.BackgroundColor = Colors.White;
            activeBtn.TextColor = Color.FromArgb("#333");
        }

        // Оновлюємо текст опису
        if (ModeDescriptionLabel != null)
        {
            ModeDescriptionLabel.Text = mode switch
            {
                0 => "🏆 Класика: Стандартна гра на час",
                1 => "🔥 Виживання: Час додається за правильні відповіді",
                2 => "👑 1 проти Всіх: Один пояснює — всі вгадують!",
                _ => ""
            };
        }
    }

    private async void LoadDecks()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("decks.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var decks = JsonSerializer.Deserialize<List<Deck>>(contents);

            // Ініціалізація, щоб не було null
            foreach (var d in decks)
            {
                d.EasyWords ??= new List<string>();
                d.MediumWords ??= new List<string>();
                d.HardWords ??= new List<string>();
                d.Words ??= new List<string>();
            }

            DecksCollection.ItemsSource = decks;
        }
        catch
        {
            DecksCollection.ItemsSource = new List<Deck> { new Deck { Name = "Error", Icon = "⚠️" } };
        }
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
                tasks.Add(particle.TranslateTo(random.Next(-50, 50), random.Next(-50, 50), (uint)random.Next(3000, 7000), Easing.SinInOut));
                tasks.Add(particle.FadeTo(random.NextDouble() * 0.4 + 0.2, (uint)random.Next(3000, 7000)));
            }
            await Task.WhenAll(tasks);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isAnimating = false;
    }
}