using SQLite;

namespace MiAppMaui.Models;

[Table("notes")]
public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = "General";

    public string Color { get; set; } = "#667eea";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsFavorite { get; set; } = false;
}
