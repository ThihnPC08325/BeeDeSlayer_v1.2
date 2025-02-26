using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LoginUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private string link = "https://phamduchuan.name.vn/PHP/Login.php"; // Link đăng nhập

    /// <summary>
    /// Hàm đăng nhập, sẽ thực thi khi người dùng nhấn nút
    /// </summary>
    public async void Login()
    {
        try
        {
            // Thu thập thông tin người dùng từ giao diện
            string username = usernameInput.text;
            string password = passwordInput.text;

            // Kiểm tra dữ liệu hợp lệ
            if (!ValidateInput(username, password))
            {
                return;
            }

            // Mã hóa mật khẩu trước khi gửi
            string encryptedPassword = EncryptionHelper.EncryptPassword(password);

            // Thực hiện gửi yêu cầu đăng nhập
            await LoginRequest(username, encryptedPassword);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }

    /// <summary>
    /// Kiểm tra input hợp lệ
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="password">Mật khẩu</param>
    /// <returns>true nếu hợp lệ, false nếu không hợp lệ</returns>
    private bool ValidateInput(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            resultText.text = "Tên đăng nhập và mật khẩu không thể để trống!";
            return false;
        }

        if (password.Length >= 6) return true;
        resultText.text = "Mật khẩu phải dài ít nhất 6 ký tự.";
        return false;
    }

    /// <summary>
    /// Hàm thực hiện đăng nhập qua HTTP request
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="encryptedPassword">Mật khẩu đã mã hóa</param>
    private async Task LoginRequest(string username, string encryptedPassword)
    {
        try
        {
            using HttpClient client = new HttpClient();
            // Thiết lập body của yêu cầu
            var values = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", encryptedPassword)
            });

            // Gửi POST request tới máy chủ
            HttpResponseMessage response = await client.PostAsync(link, values);

            // Xử lý kết quả từ máy chủ
            if (response.IsSuccessStatusCode)
            {
                string serverResponse = await response.Content.ReadAsStringAsync();

                // Xử lý phản hồi tùy theo logic server
                if (serverResponse.Contains("success"))
                {
                    resultText.text = "Đăng nhập thành công!";
                    Debug.Log("Server Response: " + serverResponse);
                }
                else
                {
                    resultText.text = $"Đăng nhập thất bại: {serverResponse}";
                    Debug.LogError("Server Error: " + serverResponse);
                }
            }
            else
            {
                resultText.text = $"Lỗi máy chủ: {response.StatusCode}";
                Debug.LogError($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            // Xử lý các lỗi kết nối hoặc lỗi không xác định
            resultText.text = $"Đã xảy ra lỗi: {ex.Message}";
            Debug.LogError($"Exception: {ex}");
        }
    }
}