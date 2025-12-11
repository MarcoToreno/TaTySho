using TaTySho.Models;

namespace TaTySho;

public static class GameState
{
    public static List<Team> Teams { get; set; } = new List<Team>();
    public static int CurrentTeamIndex { get; set; } = 0;
    public static List<string> CurrentDeck { get; set; }

    // --- НОВА ВЛАСТИВІСТЬ (Якої не вистачало) ---
    // 0 = Класика, 1 = Виживання, 2 = 1 проти Всіх
    public static int GameMode { get; set; } = 0;

    // --- НАЛАШТУВАННЯ ---
    public static int TargetScore
    {
        get => Preferences.Get("TargetScore", 30);
        set => Preferences.Set("TargetScore", value);
    }

    public static int RoundTime
    {
        get => Preferences.Get("RoundTime", 60);
        set => Preferences.Set("RoundTime", value);
    }

    public static double SoundVolume
    {
        get => Preferences.Get("SoundVolume", 1.0);
        set => Preferences.Set("SoundVolume", value);
    }

    public static bool IsDarkTheme
    {
        get => Preferences.Get("IsDarkTheme", false);
        set
        {
            Preferences.Set("IsDarkTheme", value);
            if (Application.Current != null)
                Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }
    }

    // --- ЛОГІКА ---
    public static void StartNewGame(List<Team> teams, List<string> deck)
    {
        Teams = teams;
        // Обнуляємо рахунок усім командам перед стартом
        foreach (var team in Teams)
        {
            team.TotalScore = 0;
        }

        CurrentTeamIndex = 0;
        CurrentDeck = new List<string>(deck);
    }

    public static Team GetCurrentTeam()
    {
        if (Teams == null || Teams.Count == 0) return new Team { Name = "Unknown" };
        return Teams[CurrentTeamIndex];
    }

    public static void NextTurn()
    {
        CurrentTeamIndex++;
        if (CurrentTeamIndex >= Teams.Count)
        {
            CurrentTeamIndex = 0;
        }
    }
}