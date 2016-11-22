using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class LoginScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject loginPanel;
    public Text statusText;
    public Text inputName;
    public Text initText;
    public Button createButton;
    public Button retryButton;

    bool loadUserDataSuccess;
    bool loadUserDataFail;
    bool loadHighScoreSuccess;
    bool loadHighScoreFail;
    bool loadQuizesSuccess;
    bool loadQuizesFailed;

    bool userHasId;

    void Start ()
	{
		// Get the root reference location of the database.
        Initialize();
    }

    void Update()
    {
        // Check if all data loaded
        if(loadUserDataSuccess && loadHighScoreSuccess && loadQuizesSuccess)
        {
            //Reset load flag
            ResetFlags();

            // If user already registered
            if (userHasId)
            {
                // Go to Menu
                GoToMenu();
            }
            // If not
            else
            {
                // Prompt user to create id with the createId panel
                loadingPanel.SetActive(false);
                loginPanel.SetActive(true);
            }
        }
    }

    void ResetFlags()
    {
        // Reset all flags to false, in case of re-initialize
        loadUserDataFail = false;
        loadUserDataSuccess = false;
        loadHighScoreFail = false;
        loadHighScoreSuccess = false;
        loadQuizesFailed = false;
        loadQuizesSuccess = false;
    }

    public void Initialize()
    {
        // Get & Add event to receive all data needed from server.
        FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += OnRetrieveUserData;
        FirebaseDatabase.DefaultInstance.GetReference("highscore").ValueChanged += OnRetrieveHighScore;
        FirebaseDatabase.DefaultInstance.GetReference("quizes").ValueChanged += OnRetrieveQuizes;

        // Disable retry if shown when fail
        retryButton.interactable = false;
    }

    public void CreateUser()
    {
        if (!string.IsNullOrEmpty(inputName.text))
        {
            WriteNewUser(SystemInfo.deviceUniqueIdentifier, inputName.text);
            statusText.text = "Please wait...";
            createButton.interactable = false;
        }
        else
        {
            statusText.text = "Please input your name correctly";
        }
    }

    private void WriteNewUser (string userId, string name)
	{
		User user = new User (name);
		string json = JsonUtility.ToJson (user);
        GlobalManager.instance.reference.Child("users").Child(userId).ValueChanged += OnCreateUser;
        GlobalManager.instance.reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    private void OnCreateUser(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            statusText.text = "There is some error, please try again.";
            createButton.interactable = true;
        }
        else
        {
            GlobalManager.instance.SetUserName(inputName.text);
            GoToMenu();
        }

    }

    private void OnRetrieveUserData(object sender, ValueChangedEventArgs args)
    {
       if (args.DatabaseError != null)
       {
            loadUserDataFail = true;
            // Unfortunately, firebase didn't send connection error through this..., it's just crash.
            initText.text = "There is a connection problem, please try again";
            retryButton.gameObject.SetActive(true);
            retryButton.interactable = true;
        }
       else
       {
            if (args.Snapshot.HasChild(SystemInfo.deviceUniqueIdentifier))
            {
                userHasId = true;
                if (args.Snapshot.Child(SystemInfo.deviceUniqueIdentifier).HasChild("score"))
                    GlobalManager.instance.SetUserHighScore(int.Parse(args.Snapshot.Child(SystemInfo.deviceUniqueIdentifier).Child("score").Value.ToString()));
                GlobalManager.instance.SetUserName(args.Snapshot.Child(SystemInfo.deviceUniqueIdentifier).Child("name").Value.ToString());
            }
            loadUserDataSuccess = true;
        }
    }

    private void OnRetrieveHighScore(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            loadHighScoreFail = true;
        }
        else
        {
            int globalHighScore = int.Parse(args.Snapshot.Value.ToString());
            GlobalManager.instance.SetGlobalHighScore(globalHighScore);
            loadHighScoreSuccess = true;
        }
    }

    private void OnRetrieveQuizes(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            loadHighScoreFail = true;
        }
        else
        {
            var value = args.Snapshot.GetRawJsonValue();
            var quiz = JsonUtility.FromJson<QuizList>(value);
            GlobalManager.instance.SetQuizList(quiz);
            loadQuizesSuccess = true;
        }
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}