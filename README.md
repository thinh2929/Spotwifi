# 🎵 Spotwifi - Spotify-Style Desktop Music Player

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/WPF-Windows-0078D7?style=for-the-badge&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Microsoft%20SQL%20Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/UI%20Style-Spotify%20Dark-1DB954?style=for-the-badge&logo=spotify&logoColor=white" alt="Spotify" />
</p>

---

## 📖 1. Giới thiệu dự án

**Spotwifi** là đồ án môn học **Lập trình trực quan (LTTQ)**. Ứng dụng phát và quản lý nhạc MP3 trên hệ điều hành Windows được xây dựng bằng công nghệ **WPF (.NET 8)** kết hợp với hệ quản trị cơ sở dữ liệu **Microsoft SQL Server**.

Giao diện ứng dụng được thiết kế theo phong cách **Spotify Desktop Dark Theme** hiện đại, mang lại trải nghiệm nghe nhạc mượt mà, trực quan cùng khả năng tự động đọc siêu dữ liệu (Metadata/ID3 Tags) từ các tệp MP3 cục bộ.

---

## 👥 2. Thành viên nhóm & Phân công nhiệm vụ

* **Môn học:** Lập trình trực quan (LTTQ)
* **Lớp:** `[Nhập mã lớp / Tên lớp tại đây - VD: IT008.O12]`
* **Giảng viên hướng dẫn:** `[Họ và tên Giảng viên hướng dẫn]`

| STT | Họ và tên | MSSV | Vai trò | Nhiệm vụ chính đảm nhiệm |
| :-: | :--- | :---: | :--- | :--- |
| **1** | **[Họ và tên Thành viên 1]** *(Trưởng nhóm)* | `[MSSV 1]` | **Backend & Database** | • Thiết kế CSDL SQL Server (Bảng `Songs`, `Playlists`).<br>• Module đọc Metadata & Cover Art file MP3 (`TagLibSharp`).<br>• Xây dựng tầng dữ liệu (`DatabaseHelper`, `SongService`). |
| **2** | **[Họ và tên Thành viên 2]** | `[MSSV 2]` | **WPF UI/UX Designer** | • Thiết kế giao diện phong cách Spotify Dark Theme.<br>• Bố cục 3 vùng: Sidebar, Main Content, Bottom Player Bar.<br>• Custom Styles cho Slider, ScrollViewer, Button, DataGrid. |
| **3** | **[Họ và tên Thành viên 3]** | `[MSSV 3]` | **Audio Engine & Tester/Reporter** | • Lập trình bộ máy phát nhạc (`MediaPlayer`, Shuffle, Repeat).<br>• DispatcherTimer đồng bộ thanh tua nhạc và âm lượng.<br>• Tính năng Live Search, Viết báo cáo Word & Slide thuyết trình. |

> 📌 *Kế hoạch chi tiết và tiến độ từng đầu việc được theo dõi tại file [`PHAN_CONG_NHIEM_VU.md`](PHAN_CONG_NHIEM_VU.md).*

---

## ✨ 3. Các tính năng nổi bật

- [x] **Import nhạc tiện lợi:** Nhập từng file MP3 lẻ hoặc quét toàn bộ thư mục nhạc.
- [x] **Trích xuất thông tin tự động (TagLibSharp):** Tự động đọc Tên bài hát, Nghệ sĩ, Album, Thời lượng và Ảnh bìa album (Cover Art) tích hợp sẵn trong file.
- [x] **Lưu trữ CSDL SQL Server:** Lưu danh sách bài hát, danh mục Playlist, đánh dấu bài hát yêu thích (Liked Songs) chống trùng lặp.
- [x] **Trình phát nhạc chuyên nghiệp:**
  - Phát/Tạm dừng (`Play`/`Pause`), Chuyển bài (`Next`/`Previous`).
  - Tự động chuyển bài khi kết thúc (`Auto-Next`).
  - Chế độ phát ngẫu nhiên (`Shuffle`) và lặp lại (`Repeat One` / `Repeat All`).
  - Thanh tua nhạc (Seek bar) mượt mà với bộ đếm thời gian thực tế.
  - Điều chỉnh âm lượng và tắt/bật tiếng (`Mute`/`Unmute`).
- [x] **Tìm kiếm thông minh (Live Search):** Lọc bài hát tức thời theo Tên bài, Ca sĩ hoặc Album ngay khi gõ phím.
- [x] **Giao diện Spotify Dark Theme:** Tông màu tối sang trọng (#121212, #181818) cùng điểm nhấn màu xanh Spotify (#1DB954).

---

## 🛠️ 4. Công nghệ & Thư viện sử dụng

* **Nền tảng:** Windows Presentation Foundation (WPF) trên nền **.NET 8.0**
* **Ngôn ngữ:** C# 12
* **Cơ sở dữ liệu:** Microsoft SQL Server (LocalDB / SQL Server Express)
* **Thư viện NuGet chính:**
  * [`TagLibSharp`](https://github.com/mono/taglib-sharp) (v2.3.0) — Đọc/ghi siêu dữ liệu ID3 tags & trích xuất ảnh bìa file nhạc.
  * `Microsoft.Data.SqlClient` — Kết nối và thao tác với SQL Server.

---

## 📂 5. Cấu trúc thư mục dự án

```text
Spotwifi/
│
├── Spotwifi.sln                     # Visual Studio Solution
├── PHAN_CONG_NHIEM_VU.md            # Bảng checklist phân công chi tiết
├── README.md                        # Giới thiệu & Hướng dẫn dự án
├── .gitignore                       # Cấu hình bỏ qua file rác khi đẩy lên Git
│
└── Spotwifi/                        # Thư mục mã nguồn chính
    ├── Models/                      # Các lớp thực thể (Song.cs, Playlist.cs, ...)
    ├── Views/                       # Giao diện XAML & UserControls con
    ├── Services/                    # Xử lý phát nhạc, đọc MP3 và dữ liệu
    ├── Database/                    # Script khởi tạo SQL Server & DatabaseHelper
    ├── Resources/                   # Bộ màu Spotify, Styles, Icons
    ├── MainWindow.xaml              # Giao diện cửa sổ chính
    ├── MainWindow.xaml.cs           # Code-behind xử lý tương tác chính
    └── Spotwifi.csproj              # Cấu hình dự án & gói NuGet
```

---

## 🚀 6. Hướng dẫn cài đặt & Khởi chạy dự án

### Yêu cầu hệ thống:
* Hệ điều hành: Windows 10/11
* Visual Studio 2022 (Phiên bản 17.8 trở lên, có cài gói **.NET Desktop Development**) hoặc VS Code + .NET 8 SDK
* Microsoft SQL Server (hoặc SQL Server Express / LocalDB)

### Các bước thực hiện:

1. **Clone repository về máy tính:**
   ```bash
   git clone https://github.com/<tai-khoan-cua-ban>/Spotwifi.git
   cd Spotwifi
   ```

2. **Khởi tạo Cơ sở dữ liệu:**
   - Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio.
   - Mở và chạy file script tại đường dẫn `Spotwifi/Database/Script_Tao_CSDL.sql` để tạo cơ sở dữ liệu `SpotwifiDB`.

3. **Cấu hình chuỗi kết nối (Connection String):**
   - Mở file cấu hình kết nối trong dự án và điều chỉnh lại thông tin `Server` hoặc `DataSource` phù hợp với máy của bạn.

4. **Khởi chạy ứng dụng:**
   - Mở file giải pháp `Spotwifi.sln` bằng Visual Studio 2022.
   - Nhấn **F5** hoặc chọn **Debug > Start Debugging** (hoặc chạy lệnh `dotnet run --project Spotwifi` từ terminal).

---

## 📝 7. Giấy phép & Bản quyền

Dự án này được phát triển cho mục đích học tập và nghiên cứu trong khuôn khổ môn học **Lập trình trực quan (LTTQ)**. Mọi tài nguyên âm thanh và hình ảnh phục vụ mục đích phi thương mại.
