using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private LoginData loginData;
    public void PlayGame()
    {
        if (loginData.IsLoggedIn)
        {
            SceneManager.LoadScene("Man1.1");
            Debug.Log("Play Game");
        }
        else
        {
            Debug.Log("Please login first");
        }
    }
}
