using System.Collections.ObjectModel;
using TaTySho.Models;

namespace TaTySho;

public partial class TeamPage : ContentPage
{
    public ObservableCollection<Team> Teams { get; set; } = new ObservableCollection<Team>();

    public TeamPage()
    {
        InitializeComponent();
        Teams.Add(new Team { Name = "Варенички" });
        Teams.Add(new Team { Name = "Козаки" });
        TeamsList.ItemsSource = Teams;
    }

    private void OnAddTeamClicked(object sender, EventArgs e) => Teams.Add(new Team { Name = "" });
    private void OnRemoveTeamClicked(object sender, EventArgs e) { if (Teams.Count > 2) Teams.Remove((sender as Button).CommandParameter as Team); }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        // Переходимо до вибору колоди
        int i = 1;
        foreach (var t in Teams) { if (string.IsNullOrWhiteSpace(t.Name)) t.Name = $"Команда {i}"; i++; }

        // Зберігаємо команди в GameState, але гру ще не починаємо
        GameState.Teams = Teams.ToList();

        await Shell.Current.GoToAsync(nameof(DeckPage));
    }
}