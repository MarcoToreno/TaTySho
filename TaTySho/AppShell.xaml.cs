namespace TaTySho;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // --- РЕЄСТРАЦІЯ МАРШРУТІВ ---

        // ❌ ВИДАЛЕНО: Routing.RegisterRoute(nameof(StartPage), typeof(StartPage)); 
        // (Бо StartPage вже прописана в AppShell.xaml як головна)

        // ✅ А ці сторінки треба реєструвати, бо ми на них переходимо:
        Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
        Routing.RegisterRoute(nameof(TeamPage), typeof(TeamPage));
        Routing.RegisterRoute(nameof(DeckPage), typeof(DeckPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(ResultsPage), typeof(ResultsPage));
    }
}