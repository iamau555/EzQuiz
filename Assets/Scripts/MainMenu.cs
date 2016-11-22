using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Text globalHighScore;
    public Text userHighScore;
    public Text userName;

    // Use this for initialization
    void Start ()
    {
        globalHighScore.text = "Global Best Score : " + GlobalManager.instance.GetGlobalHighScore();
        userHighScore.text = "Your Best Score : " + GlobalManager.instance.GetUserHighScore();
        userName.text = "Welcome back, " + GlobalManager.instance.GetUserName() + "!";
    }
	
	public void QuickPlay()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
