using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private static readonly string link = "https://phamduchuan.name.vn/PHP/Login.php";

    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string encryptedPassword = EncryptionHelper.EncryptPassword(password.ToString()); // For comparison
        StartCoroutine(LoginRequest(username, encryptedPassword));
    }

    private IEnumerator LoginRequest(string username, string encryptedPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", encryptedPassword);

        UnityWebRequest www = UnityWebRequest.Post(link, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            resultText.text = "Error: " + www.error;
        }
        else
        {
            resultText.text = www.downloadHandler.text;
        }
    }
}
