using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput; // Input để xác nhận mật khẩu
    [SerializeField] private static readonly string link = "https://phamduchuan.name.vn/PHP/Register.php";
    [SerializeField] private TextMeshProUGUI resultText;

    public void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Kiểm tra mật khẩu và mật khẩu xác nhận có khớp nhau không
        if (password != confirmPassword)
        {
            resultText.text = "Mật khẩu không khớp! Vui lòng kiểm tra lại";
            return; // Dừng nếu mật khẩu không khớp
        }

        // Mã hóa mật khẩu trước khi gửi lên máy chủ
        string encryptedPassword = EncryptionHelper.EncryptPassword(password);
        StartCoroutine(RegisterRequest(username, encryptedPassword));
    }

    private IEnumerator RegisterRequest(string username, string encryptedPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", encryptedPassword);

        // Gửi yêu cầu đăng ký tới máy chủ
        UnityWebRequest www = UnityWebRequest.Post(link, form);
        yield return www.SendWebRequest();

        // Kiểm tra lỗi mạng hoặc lỗi HTTP
        if (www.isNetworkError || www.isHttpError)
        {
            resultText.text = "Lỗi kết nối: " + www.error;
        }
        else
        {
            // Kiểm tra phản hồi từ máy chủ
            string response = www.downloadHandler.text;
            if (response.Contains("success"))
            {
                resultText.text = "Đăng ký thành công!";
            }
            else
            {
                resultText.text = "Đăng ký thất bại: " + response;
            }
        }
    }
}
