using UnityEngine;

[CreateAssetMenu(fileName = "LoginData", menuName = "Game/LoginData")]
public class LoginData : ScriptableObject
{
    [SerializeField] private bool isLoggedIn;
    [SerializeField] private int playerID;

    public bool IsLoggedIn => isLoggedIn;
    public int PlayerID => playerID;

    /// <summary>
    /// Đặt trạng thái đăng nhập
    /// </summary>
    public void SetLoginState(int id)
    {
        playerID = id;
        isLoggedIn = true;
    }

    /// <summary>
    /// Đăng xuất và đặt lại trạng thái
    /// </summary>
    public void Logout()
    {
        playerID = -1;
        isLoggedIn = false;
    }
}
