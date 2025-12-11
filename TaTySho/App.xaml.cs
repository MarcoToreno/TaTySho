namespace TaTySho;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        try
        {
            MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            // Цей код спробує показати помилку, якщо старт провалився
            MainPage = new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text = $"КРИТИЧНА ПОМИЛКА:\n{ex.ToString()}",
                        TextColor = Colors.Red,
                        Margin = 20
                    }
                }
            };
        }
    }
}