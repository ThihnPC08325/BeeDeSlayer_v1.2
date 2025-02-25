using System;
using System.Collections.Generic;
using System.Net.Http; // Để thay thế UnityWebRequest
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RegisterUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput; // Input để xác nhận mật khẩu
    [SerializeField] private string link = "https://phamduchuan.name.vn/PHP/Register.php"; // URL đăng ký
    [SerializeField] private TextMeshProUGUI resultText;

    /// <summary>
    /// Đăng ký người dùng
    /// </summary>
    public async void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Kiểm tra input hợp lệ trước khi gửi
        if (!ValidateInput(username, password, confirmPassword))
        {
            return;
        }

        // Mã hóa mật khẩu trước khi gửi lên máy chủ
        string encryptedPassword = EncryptionHelper.EncryptPassword(password);

        // Gọi hàm đăng ký qua HTTP
        await RegisterRequest(username, encryptedPassword);
    }

    /// <summary>
    /// Kiểm tra input hợp lệ
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="confirmPassword"></param>
    /// <returns></returns>
    private bool ValidateInput(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            resultText.text = "Tên đăng nhập và mật khẩu không thể để trống!";
            return false;
        }

        if (password != confirmPassword)
        {
            resultText.text = "Mật khẩu không khớp! Vui lòng kiểm tra lại.";
            return false;
        }

        if (password.Length < 6)
        {
            resultText.text = "Mật khẩu phải dài ít nhất 6 ký tự.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Hàm gửi yêu cầu đăng ký tới máy chủ qua HTTP
    /// </summary>
    /// <param name="username"></param>
    /// <param name="encryptedPassword"></param>
    private async Task RegisterRequest(string username, string encryptedPassword)
    {
        try
        {
            using HttpClient client = new HttpClient();
            // Dữ liệu gửi lên máy chủ dưới dạng form
            var values = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", encryptedPassword)
            });

            // Gửi POST request tới API
            HttpResponseMessage response = await client.PostAsync(link, values);

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                resultText.text = responseText.Contains("success") ? "Đăng ký thành công!" : $"Đăng ký thất bại: {responseText}";
            }
            else
            {
                resultText.text = $"Lỗi máy chủ: {response.StatusCode}";
            }
        }
        catch (Exception e)
        {
            // Bắt lỗi nếu xảy ra lỗi kết nối hoặc lỗi ngoại lệ
            resultText.text = $"Lỗi không xác định: {e.Message}";
        }
    }
}