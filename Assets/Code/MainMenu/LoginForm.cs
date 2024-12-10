using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginForm : MonoBehaviour
{
    #region URL
    private const string LoginUrl = "https://localhost:7150/api/APIGame/Login";
    #endregion

    private UIDocument _document;
    private Button _btnLogin;
    private Button _btnPlay;
    private TextField _txtUserName;
    private TextField _txtPassword;
    private bool _isLoggedIn = false; // Trạng thái đăng nhập

    private void Awake()
    {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument component is missing!");
            return;
        }

        InitializeUIElements();
        _btnLogin.clicked += OnLoginButtonClick;
        _btnPlay.clicked += OnPlayButtonClick;

        // Vô hiệu hóa nút Play ban đầu
        _btnPlay.SetEnabled(false);
    }

    private void InitializeUIElements()
    {
        var visual = _document.rootVisualElement;
        _btnLogin = visual.Q<Button>("btnLogin");
        _btnPlay = visual.Q<Button>("btnPlay"); // Thêm nút Play vào UI
        _txtUserName = visual.Q<TextField>("txtUserName");
        _txtPassword = visual.Q<TextField>("Password");
    }

    private void OnLoginButtonClick()
    {
        var loginData = new LoginRO
        {
            username = _txtUserName.value,
            password = _txtPassword.value
        };

        string jsonData = JsonUtility.ToJson(loginData);
        StartCoroutine(SendLoginRequest(jsonData));
    }

    private IEnumerator SendLoginRequest(string jsonBody)
    {
        using (var request = new UnityWebRequest(LoginUrl, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            HandleResponse(request);
        }
    }

    private void HandleResponse(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
            _isLoggedIn = false; // Đăng nhập thất bại
            _btnPlay.SetEnabled(false); // Vô hiệu hóa nút Play
            return;
        }

        var responseJson = request.downloadHandler.text;
        var loginResponse = JsonUtility.FromJson<LoginDTO>(responseJson);

        if (loginResponse != null && loginResponse.token != null)
        {
            Debug.Log("Login successful!");
            Debug.Log($"Token: {loginResponse.token}");
            _isLoggedIn = true; // Đăng nhập thành công
            _btnPlay.SetEnabled(true); // Bật nút Play
        }
        else
        {
            Debug.LogError("Login failed: " + (loginResponse?.message ?? "Unknown error"));
            _isLoggedIn = false;
            _btnPlay.SetEnabled(false);
        }
    }

    private void OnPlayButtonClick()
    {
        if (!_isLoggedIn)
        {
            Debug.LogWarning("Please log in before playing!");
            return;
        }
        gameObject.SetActive(false);
        Debug.Log("Starting the game...");
        LevelLoader.Instance.LoadLevel(GetNextSceneIndex());
    }

    private int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    [System.Serializable]
    public class LoginRO
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class Account
    {
        public string userName;
        public string password;
        public string displayName;
        public int levels;
        public int money;
    }

    [System.Serializable]
    public class LoginDTO
    {
        public string token;
        public Account account;
        public string status;
        public string message;
    }
}
