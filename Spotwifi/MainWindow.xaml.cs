using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Spotwifi.Models;

namespace Spotwifi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Song> songsList = new ObservableCollection<Song>();
        private List<Song> allSongMaster = new List<Song>();

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer progressTimer = new DispatcherTimer();

        private Song? currentSong = null;
        private bool isPlaying = false;
        private bool isUserDraggingSlider = false;

        public MainWindow()
        {
            InitializeComponent();

            DgSongs.ItemsSource = songsList;

            // Cấu hình timer cập nhật tiến trình bài hát
            progressTimer.Interval = TimeSpan.FromMilliseconds(500);
            progressTimer.Tick += ProgressTimer_Tick;

            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private string TaoMaBaiHatMoi()
        {
            if (allSongMaster.Count == 0)
                return "0001";

            int maxNumber = allSongMaster
                .Select(s => int.TryParse(s.Id, out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            return (maxNumber + 1).ToString("D4");
        }

        #region Event Handlers - Toolbar & Search

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Chọn thư mục chứa file nhạc MP3"
            };

            if (dialog.ShowDialog() == true)
            {
                string folderPath = dialog.FolderName;
                string[] files = Directory.GetFiles(folderPath, "*.mp3", SearchOption.AllDirectories);

                int countAdded = 0;
                foreach (string file in files)
                {
                    // Tránh thêm bài hát trùng đường dẫn
                    if (allSongMaster.Any(s => s.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    try
                    {
                        var tagFile = TagLib.File.Create(file);
                        var song = new Song
                        {
                            Id = TaoMaBaiHatMoi(),
                            Title = !string.IsNullOrWhiteSpace(tagFile.Tag.Title) ? tagFile.Tag.Title : Path.GetFileNameWithoutExtension(file),
                            Artist = !string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer) ? tagFile.Tag.FirstPerformer : "Unknown Artist",
                            Album = !string.IsNullOrWhiteSpace(tagFile.Tag.Album) ? tagFile.Tag.Album : "Unknown Album",
                            Duration = tagFile.Properties.Duration,
                            FilePath = file
                        };

                        // Đọc ảnh bìa nếu có
                        if (tagFile.Tag.Pictures.Length > 0)
                        {
                            byte[] bin = tagFile.Tag.Pictures[0].Data.Data;
                            var bitmap = new BitmapImage();
                            using (var mem = new MemoryStream(bin))
                            {
                                mem.Position = 0;
                                bitmap.BeginInit();
                                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = mem;
                                bitmap.EndInit();
                            }
                            bitmap.Freeze();
                            song.CoverImage = bitmap;
                        }

                        allSongMaster.Add(song);
                        songsList.Add(song);
                        countAdded++;
                    }
                    catch
                    {
                        // Bỏ qua file lỗi/không đọc được
                    }
                }

                if (countAdded > 0)
                {
                    MessageBox.Show($"Đã nhập thành công {countAdded} bài hát vào danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = TxtSearch.Text.Trim().ToLower();

            songsList.Clear();
            var filtered = string.IsNullOrEmpty(keyword)
                ? allSongMaster
                : allSongMaster.Where(s =>
                    (s.Title != null && s.Title.ToLower().Contains(keyword)) ||
                    (s.Artist != null && s.Artist.ToLower().Contains(keyword)) ||
                    (s.Album != null && s.Album.ToLower().Contains(keyword)));

            foreach (var song in filtered)
            {
                songsList.Add(song);
            }
        }

        #endregion

        #region Event Handlers - DataGrid

        private void DgSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgSongs.SelectedItem is Song selectedSong)
            {
                PlaySong(selectedSong);
            }
        }

        #endregion

        #region Event Handlers - Player Controls

        private void PlaySong(Song song)
        {
            currentSong = song;

            if (File.Exists(song.FilePath))
            {
                mediaPlayer.Open(new Uri(song.FilePath));
                mediaPlayer.Play();
                isPlaying = true;
                BtnPlayPause.Content = "⏸";

                TxtCurrentTitle.Text = song.Title;
                TxtCurrentArtist.Text = song.Artist;
                TxtTotalTime.Text = song.DurationDisplay;

                SliderProgress.Minimum = 0;
                SliderProgress.Maximum = song.Duration.TotalSeconds;
                SliderProgress.Value = 0;

                progressTimer.Start();
            }
            else
            {
                MessageBox.Show("Không tìm thấy file nhạc tại đường dẫn này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (currentSong == null)
            {
                if (songsList.Count > 0)
                {
                    PlaySong(songsList[0]);
                }
                return;
            }

            if (isPlaying)
            {
                mediaPlayer.Pause();
                isPlaying = false;
                BtnPlayPause.Content = "▶";
                progressTimer.Stop();
            }
            else
            {
                mediaPlayer.Play();
                isPlaying = true;
                BtnPlayPause.Content = "⏸";
                progressTimer.Start();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (allSongMaster.Count == 0 || currentSong == null) return;

            int currentIndex = allSongMaster.IndexOf(currentSong);
            int nextIndex = (currentIndex + 1) % allSongMaster.Count;
            PlaySong(allSongMaster[nextIndex]);
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (allSongMaster.Count == 0 || currentSong == null) return;

            int currentIndex = allSongMaster.IndexOf(currentSong);
            int prevIndex = (currentIndex - 1 + allSongMaster.Count) % allSongMaster.Count;
            PlaySong(allSongMaster[prevIndex]);
        }

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BtnNext_Click(this, new RoutedEventArgs());
            });
        }

        private void ProgressTimer_Tick(object? sender, EventArgs e)
        {
            if (!isUserDraggingSlider && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderProgress.Value = mediaPlayer.Position.TotalSeconds;
                TxtCurrentTime.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isUserDraggingSlider = true;
        }

        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUserDraggingSlider)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(SliderProgress.Value);
                TxtCurrentTime.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                isUserDraggingSlider = false;
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Volume = SliderVolume.Value;
            }
        }

        #endregion
    }
}
