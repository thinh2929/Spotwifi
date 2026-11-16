# KẾ HOẠCH & BẢNG PHÂN CÔNG NHIỆM VỤ DỰ ÁN SPOTWIFI
**Môn học:** Lập trình trực quan (LTTQ)  
**Đề tài:** Ứng dụng nghe và quản lý nhạc MP3 giao diện Spotify (WPF C# + SQL Server)  
**Quy mô nhóm:** 3 thành viên  
**Thời gian thực hiện dự kiến:** 3 - 4 tuần  

> *Ghi chú: File này sử dụng cú pháp Task List Markdown (`- [ ]`). Khi hoàn thành một task, nhóm chỉ cần sửa thành `- [x]` (hoặc tick trực tiếp trên giao diện GitHub / VS Code).*

---

## 1. TỔNG QUAN DỰ ÁN & TIẾN ĐỘ CHUNG

### 1.1. Mục tiêu sản phẩm
- [x] Khởi tạo khung dự án WPF (.NET 8.0-windows) với thư viện `TagLibSharp`.
- [x] Giải quyết các lỗi biên dịch cơ bản (Build succeeded 0 Errors, 0 Warnings).
- [ ] Import file/thư mục nhạc MP3 và tự động trích xuất metadata (ID3 tag, ảnh bìa).
- [ ] Lưu trữ và quản lý thư viện bài hát, danh sách phát (Playlist) trên SQL Server.
- [ ] Trình phát nhạc đầy đủ tính năng: Play, Pause, Next, Prev, Seek tua bài, Shuffle, Repeat, Volume.
- [ ] Thiết kế giao diện Dark Theme phong cách Spotify hiện đại (Sidebar, Danh sách nhạc, Thanh Player đáy).

---

## 2. KIẾN TRÚC HỆ THỐNG ĐỀ XUẤT (3-TIER ARCHITECTURE)

```
Spotwifi/
│
├── Models/                     # Lớp thực thể dữ liệu (Dùng chung)
│   ├── Song.cs
│   ├── Playlist.cs
│   └── PlaylistSong.cs
│
├── Database/                   # Kịch bản CSDL SQL Server (Thành viên 1)
│   ├── DatabaseHelper.cs       # Quản lý kết nối SqlConnection
│   └── Script_Tao_CSDL.sql     # Script DDL tạo Database & Bảng
│
├── Services/                   # Tầng xử lý nghiệp vụ & dữ liệu (Thành viên 1 & 3)
│   ├── SongService.cs          # CRUD bài hát trong SQL Server
│   ├── PlaylistService.cs      # CRUD Playlist & Bài hát trong Playlist
│   ├── TagLibService.cs        # Bóc tách metadata từ file MP3 & ảnh bìa
│   └── AudioPlayerService.cs   # Quản lý MediaPlayer, DispatcherTimer, Queue
│
├── Views/                      # Giao diện người dùng XAML (Thành viên 2)
│   ├── UserControls/
│   │   ├── UcSidebar.xaml      # Menu điều hướng bên trái
│   │   ├── UcPlayerBottom.xaml # Cụm Player điều khiển nhạc phía dưới
│   │   ├── UcSongList.xaml     # Bảng danh sách bài hát phong cách Spotify
│   │   ├── UcPlaylists.xaml    # Danh sách & chi tiết Playlist
│   │   └── UcSearch.xaml       # Giao diện tìm kiếm bài hát
│   └── Dialogs/
│       ├── AddPlaylistDialog.xaml
│       └── SongDetailDialog.xaml
│
└── Resources/                  # Resource phong cách Spotify (Thành viên 2)
    ├── Colors.xaml             # Bảng màu Dark Theme (#121212, #181818, #1DB954)
    ├── Styles.xaml             # Custom Slider, Button, ScrollViewer, DataGrid
    └── Icons/                  # SVG hoặc Icon PNG
```

---

## 3. CHECKLIST CHI TIẾT CHO TỪNG THÀNH VIÊN

---

### 👨‍💻 THÀNH VIÊN 1: BACKEND, CƠ SỞ DỮ LIỆU & XỬ LÝ FILE MP3
> **Vai trò:** Trưởng nhóm kỹ thuật / Phụ trách Lưu trữ & Xử lý tệp tin  
> **Trọng tâm:** SQL Server, ADO.NET (`Microsoft.Data.SqlClient`), Thư viện `TagLibSharp`.

#### Task 1.1: Thiết kế & Khởi tạo CSDL SQL Server
- [x] Cài đặt gói thư viện `Microsoft.Data.SqlClient` vào dự án (`dotnet add package Microsoft.Data.SqlClient`).
- [x] Viết file script `Database/Script_Tao_CSDL.sql` tạo CSDL `SpotwifiDB`.
- [x] Tạo bảng `Songs` (`Id`, `Title`, `Artist`, `Album`, `DurationSeconds`, `FilePath`, `CoverImagePath`, `IsFavorite`, `DateAdded`). // DateAdded: Ngày thêm nhạc vào database
- [ ] Tạo bảng `Playlists` (`PlaylistId`, `Name`, `Description`, `CreatedAt`).
- [ ] Tạo bảng `Playlist_Songs` (`PlaylistId`, `SongId`, `AddedAt`, PK composite). // AddedAt: Ngày thêm nhạc vào Playlist
- [ ] Viết lớp `Database/DatabaseHelper.cs` quản lý kết nối, mở/đóng kết nối an toàn.
- [ ] Viết các hàm tiện ích: `ExecuteQuery(sql, parameters)`, `ExecuteNonQuery(sql, parameters)`, `ExecuteScalar(sql, parameters)`.
- [ ] Thêm chuỗi kết nối `ConnectionString` vào cấu hình ứng dụng (`App.config` hoặc cấu hình tập trung).

#### Task 1.2: Module Import & Đọc Metadata MP3 (`TagLibService.cs`)
- [ ] Viết hàm mở hộp thoại chọn 1 hoặc nhiều file MP3 (`OpenFileDialog`).
- [ ] Viết hàm mở hộp thoại chọn thư mục nhạc MP3 (`OpenFolderDialog`).
- [ ] Bóc tách thông tin bài hát bằng `TagLibSharp`: Tên bài, Nghệ sĩ, Album, Thời lượng.
- [ ] Xử lý trích xuất ảnh bìa (`TagLib.IPicture`): chuyển đổi thành `BitmapImage` hoặc lưu cache ra thư mục máy.
- [ ] Xử lý trường hợp ngoại lệ: File thiếu thông tin (gán tên file làm tiêu đề, nghệ sĩ "Unknown Artist", gán ảnh bìa đĩa than mặc định).
- [ ] Tạo hàm sinh mã bài hát tự động tăng (`0001`, `0002`...).

#### Task 1.3: Tầng dịch vụ dữ liệu (`SongService.cs` & `PlaylistService.cs`)
- [ ] Viết hàm `GetAllSongs()` lấy toàn bộ bài hát từ SQL Server hiển thị lên giao diện.
- [ ] Viết hàm `InsertSong(Song song)` và `InsertSongs(List<Song> songs)` lưu vào CSDL.
- [ ] Xử lý chống thêm trùng bài hát dựa vào đường dẫn file (`FilePath`).
- [ ] Viết hàm `DeleteSong(string songId)` xóa bài hát khỏi thư viện.
- [ ] Viết hàm `ToggleFavorite(string songId, bool isFavorite)` cập nhật trạng thái yêu thích vào SQL Server.
- [ ] Viết hàm `GetFavoriteSongs()` lấy danh sách các bài hát đã thích.
- [ ] Viết hàm `CreatePlaylist(string name, string description)`.
- [ ] Viết hàm `DeletePlaylist(int playlistId)`.
- [ ] Viết hàm `AddSongToPlaylist(int playlistId, string songId)`.
- [ ] Viết hàm `RemoveSongFromPlaylist(int playlistId, string songId)`.
- [ ] Viết hàm `GetSongsByPlaylistId(int playlistId)`.

---

### 🎨 THÀNH VIÊN 2: THIẾT KẾ GIAO DIỆN SPOTIFY (WPF UI/UX DESIGNER)
> **Vai trò:** Thiết kế Giao diện & Trải nghiệm Người dùng  
> **Trọng tâm:** XAML, WPF Styles, Spotify Dark Theme, Custom Controls.

#### Task 2.1: Xây dựng hệ thống Spotify Theme Resource
- [ ] Tạo file `Resources/Colors.xaml` định nghĩa bảng màu chuẩn Spotify:
  - Nền chính: `#121212`
  - Nền thẻ (Card) / Sidebar: `#181818`, Hover: `#282828`
  - Điểm nhấn xanh Spotify: `#1DB954`, Hover: `#1ED760`
  - Màu chữ chính: `#FFFFFF`, Màu chữ phụ: `#B3B3B3`
- [ ] Tạo file `Resources/Styles.xaml`:
  - [ ] Custom `ScrollViewer`: Thanh cuộn mỏng, màu tối hiện đại, ẩn mũi tên.
  - [ ] Custom `Slider` cho Seek Bar (thanh tiến trình nhạc): Đường line mỏng, chấm tròn thumb đổi màu khi rê chuột.
  - [ ] Custom `Slider` cho Volume: Thanh trượt nhỏ gọn, màu xanh lá khi kéo.
  - [ ] Custom `Button`: Bo góc tròn (CornerRadius 20px), hiệu ứng hover đổi màu nền.
  - [ ] Custom `TextBox`: Khung bo tròn, nền xám tối, chữ trắng, placeholder mờ.

#### Task 2.2: Thiết kế bố cục chính `MainWindow.xaml`
- [ ] Thiết kế thanh tiêu đề ứng dụng (Window Header): Nút Thu nhỏ, Phóng to, Đóng theo phong cách phẳng (Flat).
- [ ] Thiết kế **Sidebar bên trái** (`UcSidebar.xaml`):
  - [ ] Logo Spotwifi + Icon nốt nhạc.
  - [ ] Cụm menu chính: Trang chủ (Home), Tìm kiếm (Search), Thư viện (Your Library).
  - [ ] Cụm chức năng: Nút "Tạo Playlist", Mục "Bài hát yêu thích" (Liked Songs - icon trái tim gradient tím/xanh).
  - [ ] Vùng danh sách các Playlist đã tạo của người dùng (hỗ trợ cuộn chuột mượt mà).
- [ ] Thiết kế **Vùng nội dung chính (Main Content Area)**:
  - [ ] Tạo vùng chứa linh hoạt (`ContentControl` hoặc `Frame`) để chuyển đổi giữa các màn hình con.
- [ ] Thiết kế **Thanh Player cố định phía dưới** (`UcPlayerBottom.xaml`):
  - [ ] Vùng trái: Ảnh bìa vuông nhỏ (56x56), Tên bài hát, Tên nghệ sĩ, Nút Trái tim (Like/Unlike).
  - [ ] Vùng giữa: Cụm nút Play/Pause tròn lớn, Nút Prev, Nút Next, Nút Shuffle, Nút Repeat; Thanh tua nhạc (Seek bar) cùng nhãn thời gian 2 đầu (`01:25` - `03:45`).
  - [ ] Vùng phải: Icon loa âm thanh, Thanh trượt Volume, Nút Mute.

#### Task 2.3: Thiết kế các màn hình chi tiết (UserControls & Dialogs)
- [ ] Thiết kế `UcSongList.xaml` (Bảng danh sách bài hát):
  - [ ] Header bảng: `#`, Tiêu đề & Ca sĩ, Album, Ngày thêm, Thời lượng.
  - [ ] Item template: Dòng hiển thị hover đổi màu `#282828`, số thứ tự chuyển thành nút Play khi hover.
  - [ ] Nút chọn file/thư mục MP3 đặt ở vị trí trực quan trên đầu trang.
- [ ] Thiết kế `UcPlaylists.xaml` (Chi tiết Playlist):
  - [ ] Header lớn có Gradient đổi màu, ảnh bìa lớn (180x180), Tên playlist, số lượng bài hát, tổng thời lượng.
  - [ ] Nút tròn xanh lá "Play All" lớn đặc trưng của Spotify.
  - [ ] Bảng danh sách các bài hát trong playlist.
- [ ] Thiết kế `UcSearch.xaml` (Trang tìm kiếm):
  - [ ] Thanh Search lớn ở đầu trang với icon kính lúp.
  - [ ] Danh sách kết quả tìm kiếm hiển thị dạng lưới hoặc bảng.
- [ ] Thiết kế Dialog & ContextMenu:
  - [ ] Popup Dialog "Tạo Playlist mới" (Nhập tên playlist, mô tả).
  - [ ] ContextMenu chuột phải khi click vào bài hát: "Phát bài", "Thêm vào Playlist", "Yêu thích", "Xóa".

---

### 🎵 THÀNH VIÊN 3: TRÌNH PHÁT NHẠC, TÍNH NĂNG TƯƠNG TÁC & BÁO CÁO
> **Vai trò:** Lập trình Logic Phát nhạc, Xử lý Tương tác & Đảm bảo Chất lượng  
> **Trọng tâm:** `System.Windows.Media.MediaPlayer`, `DispatcherTimer`, Tìm kiếm, Báo cáo & Thuyết trình.

#### Task 3.1: Xây dựng bộ máy phát nhạc (`AudioPlayerService.cs`)
- [x] Khởi tạo và quản lý đối tượng `MediaPlayer`.
- [x] Viết hàm `PlaySong(Song song)`: Nạp file từ đường dẫn `FilePath` và phát nhạc.
- [x] Viết hàm `Pause()` và `Resume()`: Chuyển đổi trạng thái nút Play/Pause trên giao diện.
- [ ] Viết hàm `Next()`: Chuyển sang bài tiếp theo trong hàng đợi (Queue).
- [ ] Viết hàm `Previous()`: Quay lại bài hát trước đó.
- [ ] Xử lý sự kiện `MediaEnded`: Tự động chuyển phát bài kế tiếp khi bài hiện tại chạy hết.
- [ ] Viết thuật toán phát ngẫu nhiên (`Shuffle`): Trộn danh sách bài hát không trùng lặp.
- [ ] Viết chế độ lặp lại (`Repeat`):
  - [ ] Lặp lại toàn bộ danh sách (Repeat All).
  - [ ] Lặp lại 1 bài duy nhất (Repeat One).
  - [ ] Tắt lặp lại (No Repeat).

#### Task 3.2: Đồng bộ thanh tiến trình phát nhạc (Seek bar) & Âm lượng
- [ ] Cấu hình `DispatcherTimer` chạy ngầm (chu kỳ 500ms) để cập nhật vị trí bài hát.
- [ ] Đồng bộ giá trị `mediaPlayer.Position` lên thanh trượt `SliderProgress`.
- [ ] Cập nhật TextBlock thời gian hiện tại (`TxtCurrentTime`) và tổng thời lượng (`TxtTotalTime`).
- [ ] Bắt sự kiện kéo tua nhạc (`Slider_PreviewMouseDown`, `SliderProgress_ValueChanged`):
  - Tạm dừng timer khi người dùng đang giữ chuột kéo thanh tua.
  - Áp dụng thời gian tua mới vào `mediaPlayer.Position` khi nhả chuột.
- [ ] Bắt sự kiện thanh âm lượng (`SliderVolume_ValueChanged`): Thay đổi `mediaPlayer.Volume` (từ 0.0 đến 1.0).
- [ ] Viết tính năng Bật/Tắt tiếng (Mute/Unmute): Lưu lại mức âm lượng trước khi bấm mute để phục hồi khi unmute.

#### Task 3.3: Tính năng tương tác & Tìm kiếm thời gian thực
- [ ] Xử lý sự kiện Double-click vào dòng bài hát trên bảng (`DgSongs_MouseDoubleClick`) để phát bài ngay lập tức.
- [ ] Lập trình Live Search (`TxtSearch_TextChanged`):
  - Lọc danh sách bài hát theo Tên bài hát, Nghệ sĩ hoặc Album ngay khi người dùng gõ ký tự.
  - Cập nhật lại `ObservableCollection<Song>` hiển thị mà không gây giật lag.
- [ ] Xử lý phím tắt cơ bản (Space: Play/Pause, Mũi tên trái/phải: Tua 5s, Mũi tên lên/xuống: Âm lượng).

#### Task 3.4: Kiểm thử ứng dụng, Viết Báo cáo & Slide thuyết trình
- [ ] **Kiểm thử hệ thống (Testing):**
  - [ ] Test import thư mục lớn (>50 bài hát).
  - [ ] Test file MP3 có ký tự tiếng Việt có dấu, file đường dẫn dài.
  - [ ] Test các file MP3 không có tag ID3, không có ảnh bìa.
  - [ ] Test trường hợp file MP3 bị di chuyển hoặc xóa khỏi ổ đĩa.
  - [ ] Test các chế độ phát: Shuffle, Repeat One, Repeat All.
- [ ] **Viết Báo cáo Đồ án (Word/PDF):**
  - [ ] Trang bìa, Lời cảm ơn, Mục lục tự động.
  - [ ] Giới thiệu đề tài, lý do chọn đề tài, khảo sát ứng dụng Spotify thực tế.
  - [ ] Cơ sở lý thuyết: C#, WPF, XAML, SQL Server, TagLibSharp, MediaPlayer.
  - [ ] Phân tích & Thiết kế hệ thống: Sơ đồ ca sử dụng (Use Case), Thiết kế CSDL (ERD, Lược đồ quan hệ), Sơ đồ lớp (Class Diagram).
  - [ ] Hiện thực hóa chức năng & Giao diện (kèm ảnh chụp màn hình minh họa).
  - [ ] Kết luận, hướng phát triển và tài liệu tham khảo.
- [ ] **Thiết kế Slide thuyết trình PowerPoint:**
  - [ ] Tạo template slide tone màu Dark Spotify chuyên nghiệp.
  - [ ] Tổng hợp các tính năng nổi bật, kiến trúc hệ thống, video/ảnh chụp demo.
  - [ ] Chuẩn bị kịch bản thuyết trình và phân chia thuyết trình cho 3 người.

---

## 4. MA TRẬN PHỐI HỢP & GIAO TIẾP DỮ LIỆU (INTERFACE CONTRACT)

```
[Thành viên 1: Backend/DB]
       │
       ▼ (Cung cấp ObservableCollection<Song> & các hàm CRUD)
[Thành viên 3: Player Logic & Filter]
       ▲
       │ (Gắn sự kiện Click, DoubleClick, SliderValueChanged)
[Thành viên 2: Spotify UI/UX]
```

### Quy ước dữ liệu chung (`Models/Song.cs`):
```csharp
public class Song
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = "Unknown Artist";
    public string Album { get; set; } = "Unknown Album";
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public BitmapImage? CoverImage { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Now;

    public string DurationDisplay => Duration.ToString(@"mm\:ss");
}
```

---

## 5. LỘ TRÌNH TIẾN ĐỘ 4 TUẦN (TIMELINE CHECKLIST)

### Tuần 1: Setup dự án, Sửa lỗi build & Khởi tạo CSDL
- [x] Sửa triệt để 9 lỗi biên dịch ban đầu trong `MainWindow.xaml.cs`.
- [x] Tạo file `.gitignore` và thiết lập kho Git / GitHub.
- [ ] Thành viên 1: Cài `Microsoft.Data.SqlClient`, tạo CSDL `SpotwifiDB` và `DatabaseHelper.cs`.
- [ ] Thành viên 2: Tạo `Colors.xaml`, `Styles.xaml` phong cách Spotify Dark Theme.
- [ ] Thành viên 3: Khởi tạo khung `AudioPlayerService.cs`, thử nghiệm phát nhạc MP3 cơ bản.

### Tuần 2: Xử lý MP3, Lưu trữ CSDL & Dựng giao diện chính
- [ ] Thành viên 1: Hoàn thiện `TagLibService.cs` bóc tách tag MP3 & hàm lưu bài hát vào CSDL.
- [ ] Thành viên 2: Hoàn thiện giao diện Sidebar, bảng bài hát chuẩn Spotify và Bottom Player bar.
- [ ] Thành viên 3: Hoàn thiện Timer cập nhật Seek Bar, Play/Pause/Next/Prev, thanh âm lượng.

### Tuần 3: Quản lý Playlist, Tìm kiếm & Tính năng nâng cao
- [ ] Thành viên 1: Hoàn thiện chức năng Playlist (CRUD Playlist & thêm bài vào playlist).
- [ ] Thành viên 2: Thiết kế màn hình Playlist chi tiết, trang Tìm kiếm và Popup tạo Playlist.
- [ ] Thành viên 3: Tích hợp Live Search, tính năng Shuffle, Repeat và double-click phát nhạc.

### Tuần 4: Kiểm thử, Tinh chỉnh giao diện & Hoàn thiện báo cáo
- [ ] Thành viên 1: Tối ưu truy vấn SQL Server, xử lý ngoại lệ file.
- [ ] Thành viên 2: Tinh chỉnh hiệu ứng hover, font chữ, pixel-perfect chuẩn phong cách Spotify.
- [ ] Thành viên 3: Kiểm thử toàn diện các luồng lỗi, hoàn thiện File Báo cáo Word và Slide thuyết trình.
