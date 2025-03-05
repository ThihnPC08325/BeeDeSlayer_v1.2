using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Newtonsoft.Json.Linq; // Dùng để parse JSON

public class LoginUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private LoginData loginData;
    private static readonly string link = "https://phamduchuan.name.vn/PHP/Login.php";

    /// <summary>
    /// Hàm đăng nhập, sẽ thực thi khi người dùng nhấn nút
    /// </summary>
    public async void Login()
    {
        try
        {
            string username = usernameInput.text;
            string password = passwordInput.text;

            if (!ValidateInput(username, password))
            {
                return;
            }

            string encryptedPassword = EncryptionHelper.EncryptPassword(password);
            await LoginRequest(username, encryptedPassword);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }

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

    private async Task LoginRequest(string username, string encryptedPassword)
    {
        try
        {
            using HttpClient client = new HttpClient();
            var values = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", encryptedPassword)
            });

            HttpResponseMessage response = await client.PostAsync(link, values);
            string serverResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("Server Response: " + serverResponse);

                // Parse JSON từ phản hồi của server
                JObject jsonResponse = JObject.Parse(serverResponse);
                string status = jsonResponse["status"]?.ToString();

                if (status == "success")
                {
                    int playerID = jsonResponse["Player_ID"]?.Value<int>() ?? -1;
                    resultText.text = $"Đăng nhập thành công!";

                    // Lưu trạng thái vào ScriptableObject
                    loginData.SetLoginState(playerID);
                }
                else
                {
                    resultText.text = $"Đăng nhập thất bại: {jsonResponse["message"]}";
                }
            }
            else
            {
                resultText.text = $"Lỗi máy chủ: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            resultText.text = $"Đã xảy ra lỗi: {ex.Message}";
            Debug.LogError($"Exception: {ex}");
        }
    }

    public void Logout()
    {
        loginData.Logout();
        resultText.text = "Đăng xuất thành công!";
    }
}
