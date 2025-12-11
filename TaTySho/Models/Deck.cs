using Microsoft.Maui.Graphics;
using System.Text.Json.Serialization;

namespace TaTySho.Models;

public class Deck
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }

    // 1. ПОВЕРТАЄМО Words, щоб код не ламався
    public List<string> Words { get; set; } = new List<string>();

    // Списки по складності
    public List<string> EasyWords { get; set; } = new List<string>();
    public List<string> MediumWords { get; set; } = new List<string>();
    public List<string> HardWords { get; set; } = new List<string>();

    // 2. Властивості для JSON (зберігаємо кольори як текст)
    public string HexStart { get; set; }
    public string HexEnd { get; set; }

    // 3. Властивості для Коду (тепер вони "розумні")
    [JsonIgnore]
    public Color StartColor
    {
        get
        {
            // Якщо є Hex-код з файлу - беремо його, інакше - беремо те, що задали вручну
            if (!string.IsNullOrEmpty(HexStart)) return Color.FromArgb(HexStart);
            return _startColor;
        }
        set => _startColor = value; // Дозволяємо записувати вручну!
    }
    private Color _startColor = Colors.Gray; // Колір за замовчуванням

    [JsonIgnore]
    public Color EndColor
    {
        get
        {
            if (!string.IsNullOrEmpty(HexEnd)) return Color.FromArgb(HexEnd);
            return _endColor;
        }
        set => _endColor = value;
    }
    private Color _endColor = Colors.Black;
}