using System.Text.Json;
using TaTySho.Models;

namespace TaTySho;

public partial class DeckPage : ContentPage
{
    public DeckPage()
    {
        InitializeComponent();
        LoadDecks();
    }

    private async void LoadDecks()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("decks.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var decks = JsonSerializer.Deserialize<List<Deck>>(contents);
            DecksCollection.ItemsSource = decks;
        }
        catch
        {
            await DisplayAlert("Помилка", "Не вдалося завантажити теми", "Ок");
        }
    }

    private async void OnDeckTapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        await frame.ScaleTo(0.95, 100); await frame.ScaleTo(1.0, 100);

        var deck = e.Parameter as Deck;

        string difficulty = await DisplayActionSheet($"Складність '{deck.Name}':", "Скасувати", null, "🟢 Легко", "🟡 Середньо", "🔴 Важко");
        if (difficulty == "Скасувати" || difficulty == null) return;

        List<string> words = new List<string>();
        if (difficulty == "🟢 Легко") words = deck.EasyWords;
        else if (difficulty == "🟡 Середньо") words = deck.MediumWords;
        else if (difficulty == "🔴 Важко") words = deck.HardWords;

        if (words == null || words.Count == 0) words = deck.Words; // Фолбек

        // ФІНАЛЬНИЙ СТАРТ
        // Команди вже в GameState.Teams, просто обнуляємо рахунок
        foreach (var t in GameState.Teams) t.TotalScore = 0;
        GameState.CurrentTeamIndex = 0;
        GameState.CurrentDeck = words;

        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}