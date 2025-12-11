using Microsoft.Maui.Layouts;

namespace TaTySho;

public partial class ResultsPage : ContentPage
{
    // Список завдань для тих, хто програв
    private List<string> punishments = new List<string>
    {
        "Присісти 10 разів",
        "Розповісти віршик як у садочку",
        "Гавкати у вікно",
        "Зробити масаж плечей переможцям",
        "Затанцювати гопака",
        "Написати колишній/колишньому",
        "Кукурікати 3 рази",
        "Зобразити тиранозавра",
        "Говорити тільки пошепки наступний раунд"
    };

    public ResultsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var team1 = GameState.Teams[0];
        var team2 = GameState.Teams[1];
        Team1NameLabel.Text = team1.Name;
        Team1ScoreLabel.Text = team1.TotalScore.ToString();
        Team2NameLabel.Text = team2.Name;
        Team2ScoreLabel.Text = team2.TotalScore.ToString();

        // Скидаємо покарання (ховаємо картку)
        PunishmentFrame.IsVisible = false;

        // Перевірка перемоги
        if (team1.TotalScore >= GameState.TargetScore || team2.TotalScore >= GameState.TargetScore)
        {
            string winnerName = team1.TotalScore > team2.TotalScore ? team1.Name : team2.Name;

            // Знаходимо того, хто програв (найменше балів)
            var loser = GameState.Teams.OrderBy(t => t.TotalScore).First();

            NextTeamLabel.Text = $"🏆 ПЕРЕМОГА: {winnerName}!";
            NextTeamLabel.TextColor = Colors.Gold;

            // --- ЛОГІКА ПОКАРАННЯ ---
            var rnd = new Random();
            string task = punishments[rnd.Next(punishments.Count)];
            PunishmentLabel.Text = $"{loser.Name}, вам треба:\n{task}";
            PunishmentFrame.IsVisible = true; // Показуємо картку!
            // ------------------------

            NextRoundBtn.Text = "Нова гра";
            NextRoundBtn.Clicked -= OnNextRoundClicked;
            NextRoundBtn.Clicked += OnMenuClicked;

            await RunConfetti();
        }
        else
        {
            GameState.NextTurn();
            var nextTeam = GameState.GetCurrentTeam();
            NextTeamLabel.Text = nextTeam.Name;
            NextTeamLabel.TextColor = Colors.Black;
        }
    }

    private async Task RunConfetti()
    {
        var random = new Random();
        var colors = new[] { Colors.Red, Colors.Gold, Colors.Blue, Colors.Lime, Colors.Purple, Colors.Cyan };

        for (int i = 0; i < 100; i++)
        {
            var box = new BoxView
            {
                Color = colors[random.Next(colors.Length)],
                WidthRequest = 10,
                HeightRequest = 10,
                CornerRadius = 5
            };

            double startX = random.NextDouble();
            AbsoluteLayout.SetLayoutBounds(box, new Rect(startX, -0.1, 10, 10));
            AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.PositionProportional);
            ConfettiLayout.Children.Add(box);

            _ = box.TranslateTo(random.Next(-100, 100), 800, (uint)random.Next(2000, 4000), Easing.Linear);
            _ = box.RotateTo(random.Next(180, 720), (uint)random.Next(2000, 4000));
            if (i % 5 == 0) await Task.Delay(10);
        }
    }

    private async void OnNextRoundClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync(nameof(MainPage));
    private async void OnMenuClicked(object sender, EventArgs e) => await Shell.Current.Navigation.PopToRootAsync();
}