using System;
using System.Windows.Media.Imaging;

namespace Spotwifi.Models
{
    public class Song
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = "Unknown Artist";
        public string Album { get; set; } = "Unknown Album";
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public BitmapImage? CoverImage { get; set; }
        public bool IsFavorite { get; set; } = false;
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public string DurationDisplay => Duration.ToString(@"mm\:ss");
    }
}