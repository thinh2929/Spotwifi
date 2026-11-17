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
    public partial class MainWindow : Window
    {
        private ObservableCollection<Song> songsList = new ObservableCollection<Song>();
        private List<Song> allSongMaster = new List<Song>();

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer progressTimer = new DispatcherTimer();

        private Song? currentSong = null;
        private bool isPlaying = false;
        private bool isUserDraggingSlider = false;
        private bool isSidebarOpen = true;

        private bool isShuffle = false;
        private List<Song> shuffledSongs = new List<Song>();
        private Random rng = new Random();
        public MainWindow()
        {
            InitializeComponent();

            DgSongs.ItemsSource = songsList;

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

        #region Sidebar

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            isSidebarOpen = !isSidebarOpen;
            SidebarColumn.Width = isSidebarOpen
                ? new GridLength(280)
                : new GridLength(0);
            BtnToggleSidebar.Content = isSidebarOpen ? "◀" : "▶";
        }

        private void BtnTabInfo_Click(object sender, RoutedEventArgs e)
        {
            PanelSongInfo.Visibility = Visibility.Visible;
            PanelQueue.Visibility = Visibility.Collapsed;

            BtnTabInfo.Foreground = Brushes.Black;
            BtnTabInfo.BorderThickness = new Thickness(0, 0, 0, 2);
            BtnTabInfo.BorderBrush = Brushes.Black;

            BtnTabQueue.Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D));
            BtnTabQueue.BorderThickness = new Thickness(0);
        }

        private void BtnTabQueue_Click(object sender, RoutedEventArgs e)
        {
            PanelSongInfo.Visibility = Visibility.Collapsed;
            PanelQueue.Visibility = Visibility.Visible;

            BtnTabQueue.Foreground = Brushes.Black;
            BtnTabQueue.BorderThickness = new Thickness(0, 0, 0, 2);
            BtnTabQueue.BorderBrush = Brushes.Black;

            BtnTabInfo.Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D));
            BtnTabInfo.BorderThickness = new Thickness(0);
        }

        private void LbQueue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LbQueue.SelectedItem is Song song)
                PlaySong(song);
        }

        private void UpdateSidebar(Song song)
        {
            // Cập nhật tab "Đang phát"
            TxtSidebarTitle.Text = song.Title;
            TxtSidebarArtist.Text = song.Artist;
            TxtSidebarAlbum.Text = song.Album;
            TxtSidebarDuration.Text = song.DurationDisplay;
            TxtSidebarDateAdded.Text = song.DateAdded.ToString("dd/MM/yyyy");
            ImgSidebarCover.Source = song.CoverImage;

            // --- THÊM 3 DÒNG NÀY: Cập nhật khối bài đang phát trên đầu của tab Queue ---
            TxtQueueCurrentTitle.Text = song.Title;
            TxtQueueCurrentArtist.Text = song.Artist;
            ImgQueueCurrentCover.Source = song.CoverImage;

            // Cập nhật các bài tiếp theo trong hàng đợi
            int idx = allSongMaster.IndexOf(song);
            LbQueue.ItemsSource = allSongMaster.Skip(idx + 1).ToList();
        }

        private void CapNhatQueueSidebar(Song song)
        {
            var targetList = isShuffle && shuffledSongs.Count > 0 ? shuffledSongs : allSongMaster;
            int idx = targetList.IndexOf(song);
            if (idx != -1)
            {
                LbQueue.ItemsSource = targetList.Skip(idx + 1).ToList();
            }
        }

        #endregion

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
                    MessageBox.Show($"Đã nhập thành công {countAdded} bài hát!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                songsList.Add(song);
        }

        #endregion

        #region Event Handlers - DataGrid

        private void DgSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgSongs.SelectedItem is Song selectedSong)
                PlaySong(selectedSong);
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

                // Cập nhật Player bar
                TxtCurrentTitle.Text = song.Title;
                TxtCurrentArtist.Text = song.Artist;
                TxtTotalTime.Text = song.DurationDisplay;

                SliderProgress.Minimum = 0;
                SliderProgress.Maximum = song.Duration.TotalSeconds;
                SliderProgress.Value = 0;

                progressTimer.Start();

                // Cập nhật Sidebar
                UpdateSidebar(song);
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
                    PlaySong(songsList[0]);
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

            // Chọn danh sách đang áp dụng: ngẫu nhiên hay danh sách gốc
            var targetList = isShuffle && shuffledSongs.Count > 0 ? shuffledSongs : allSongMaster;

            int currentIndex = targetList.IndexOf(currentSong);
            if (currentIndex == -1) currentIndex = 0;

            int nextIndex = (currentIndex + 1) % targetList.Count;
            PlaySong(targetList[nextIndex]);
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (allSongMaster.Count == 0 || currentSong == null) return;

            var targetList = isShuffle && shuffledSongs.Count > 0 ? shuffledSongs : allSongMaster;

            int currentIndex = targetList.IndexOf(currentSong);
            if (currentIndex == -1) currentIndex = 0;

            int prevIndex = (currentIndex - 1 + targetList.Count) % targetList.Count;
            PlaySong(targetList[prevIndex]);
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
                mediaPlayer.Volume = SliderVolume.Value;
        }

        #endregion

        #region Shuffle Logic

        /// <summary>
        /// Trộn danh sách bài hát theo thuật toán Fisher-Yates (không trùng lặp)
        /// </summary>
        private void TaoDanhSachShuffle()
        {
            if (allSongMaster.Count == 0) return;

            shuffledSongs = new List<Song>(allSongMaster);

            // Nếu đang có bài hát phát, tách riêng bài đó ra đưa lên đầu danh sách
            if (currentSong != null)
            {
                shuffledSongs.Remove(currentSong);
            }

            // Thuật toán Fisher-Yates trộn từ cuối lên đầu
            int n = shuffledSongs.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                // Hoán vị 2 phần tử
                Song temp = shuffledSongs[k];
                shuffledSongs[k] = shuffledSongs[n];
                shuffledSongs[n] = temp;
            }

            // Đưa bài đang phát vào lại vị trí đầu tiên
            if (currentSong != null)
            {
                shuffledSongs.Insert(0, currentSong);
            }
        }

        private void BtnShuffle_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = !isShuffle;

            if (isShuffle)
            {
                TaoDanhSachShuffle();
                // Đổi màu nút để báo hiệu đang BẬT (màu đen hoặc xanh)
                BtnShuffle.Foreground = Brushes.Black;
                BtnShuffle.FontWeight = FontWeights.Bold;
            }
            else
            {
                shuffledSongs.Clear();
                // Trả màu nút về mặc định (xám)
                BtnShuffle.Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D));
                BtnShuffle.FontWeight = FontWeights.Normal;
            }

            // Cập nhật lại tab Danh sách chờ (Queue) ở Sidebar phải
            if (currentSong != null)
            {
                CapNhatQueueSidebar(currentSong);
            }
        }

        #endregion
    }
}
