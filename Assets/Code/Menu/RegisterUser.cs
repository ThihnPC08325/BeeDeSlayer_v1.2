using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RegisterUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    [SerializeField] private static readonly string link = "https://phamduchuan.name.vn/PHP/Register.php";
    [SerializeField] private TextMeshProUGUI resultText;

    /// <summary>
    /// Đăng ký người dùng
    /// </summary>
    public async void Register()
    {
        try
        {
            string username = usernameInput.text.Trim();
            string password = passwordInput.text;
            string confirmPassword = confirmPasswordInput.text;

            // Kiểm tra input hợp lệ
            if (!ValidateInput(username, password, confirmPassword))
                return;

            // Gửi request đăng ký
            await RegisterRequest(username, password);
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi ngoại lệ: {e}");
        }
    }

    /// <summary>
    /// Kiểm tra input hợp lệ
    /// </summary>
    private bool ValidateInput(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            resultText.text = "Tên đăng nhập và mật khẩu không thể để trống!";
            return false;
        }

        if (password.Length < 6)
        {
            resultText.text = "Mật khẩu phải dài ít nhất 6 ký tự!";
            return false;
        }

        if (password != confirmPassword)
        {
            resultText.text = "Mật khẩu không khớp!";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gửi yêu cầu đăng ký tới máy chủ
    /// </summary>
    private async Task RegisterRequest(string username, string password)
    {
        try
        {
            using HttpClient client = new HttpClient();

            var data = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await client.PostAsync(link, content);

            string responseText = await response.Content.ReadAsStringAsync();
            Debug.Log($"Server Response: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JsonUtility.FromJson<ServerResponse>(responseText);

                if (jsonResponse.status == "success")
                {
                    resultText.text = "Đăng ký thành công! Đăng nhập để vào game.";
                }
                else
                {
                    resultText.text = $"Lỗi: {jsonResponse.message}";
                }
            }
            else
            {
                resultText.text = $"Lỗi máy chủ: {response.StatusCode}";
            }
        }
        catch (Exception e)
        {
            resultText.text = $"Lỗi kết nối: {e.Message}";
        }
    }

    /// <summary>
    /// Lớp hỗ trợ parse JSON từ máy chủ
    /// </summary>
    [Serializable]
    private class ServerResponse
    {
        public string status;
        public string message;
    }
}
