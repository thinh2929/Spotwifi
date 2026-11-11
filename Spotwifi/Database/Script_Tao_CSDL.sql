-- Tạo Database SpotwifiDB
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SpotwifiDB')
BEGIN
    CREATE DATABASE SpotwifiDB;
END
GO

USE SpotwifiDB;
GO

-- 1. Bảng lưu trữ bài hát
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Songs]') AND type in (N'U'))
BEGIN
    CREATE TABLE Songs (
        Id NVARCHAR(10) PRIMARY KEY,              -- Mã định dạng '0001', '0002'...
        Title NVARCHAR(255) NOT NULL,
        Artist NVARCHAR(255) DEFAULT N'Unknown Artist',
        Album NVARCHAR(255) DEFAULT N'Unknown Album',
        DurationSeconds INT NOT NULL DEFAULT 0,
        FilePath NVARCHAR(1000) NOT NULL UNIQUE,
        CoverImagePath NVARCHAR(1000) NULL,
        IsFavorite BIT NOT NULL DEFAULT 0,
        DateAdded DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. Bảng Playlists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Playlists]') AND type in (N'U'))
BEGIN
    CREATE TABLE Playlists (
        PlaylistId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 3. Bảng liên kết Playlist và Songs (Quan hệ N - N)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Playlist_Songs]') AND type in (N'U'))
BEGIN
    CREATE TABLE Playlist_Songs (
        PlaylistId INT NOT NULL,
        SongId NVARCHAR(10) NOT NULL,
        AddedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_Playlist_Songs PRIMARY KEY (PlaylistId, SongId),
        CONSTRAINT FK_PlaylistSongs_Playlists FOREIGN KEY (PlaylistId) REFERENCES Playlists(PlaylistId) ON DELETE CASCADE,
        CONSTRAINT FK_PlaylistSongs_Songs FOREIGN KEY (SongId) REFERENCES Songs(Id) ON DELETE CASCADE
    );
END
GO