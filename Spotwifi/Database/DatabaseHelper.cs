using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Spotwifi.Database
{
    public static class DatabaseHelper
    {
        // Chuỗi kết nối Windows Authentication (localhost)
        // Chỉnh sửa tên server nếu máy bạn dùng instance riêng như .\SQLEXPRESS
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SpotwifiDB"]?.ConnectionString
            ?? throw new InvalidOperationException(
                "Chua cau hinh ConnectionString SpotwifiDB trong App.config!"
            );

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Thực thi câu lệnh INSERT, UPDATE, DELETE
        public static int ExecuteNonQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // Lấy về bảng dữ liệu (SELECT)
        public static DataTable ExecuteQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // Lấy về 1 giá trị đơn lẻ (COUNT, MAX, EXIST...)
        public static object? ExecuteScalar(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            return cmd.ExecuteScalar();
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                using var conn = GetConnection();

                conn.Open();

                using var cmd = new SqlCommand(
                    "SELECT DB_NAME()", conn);

                string databaseName =
                    Convert.ToString(cmd.ExecuteScalar()) ?? "";

                message = $"Kết nối thành công! Database: {databaseName}";

                return true;
            }
            catch (Exception ex)
            {
                message = "Kết nối thất bại: " + ex.Message;

                return false;
            }
        }
    }
}