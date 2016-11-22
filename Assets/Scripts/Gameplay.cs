using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class Gameplay : MonoBehaviour {

    List<int> SelectedQuestions = new List<int>();

    public GameObject readyPanel;
    public GameObject quizPanel;
    public GameObject summaryPanel;

    // Ready Panel
    public Text countdown;

    // Quiz Panel
    public Image heart1;
    public Image heart2;
    public Text roundNumber;
    public Text question;
    public Button nextButton;
    public Button choice1;
    public Button choice2;
    public Button choice3;
    public Button choice4;
    public Scrollbar timeBar;

    public Text nextText;
    public Text scorePlusText;

    // Summary Panel;
    public Text highScore;
    public Text yourScore;
    public Text correctedAnswer;
    public Text heartLeft;
    public Text updatingText;

    public Button retryButton;
    public Button menuButton;

    string userName;

    // Use this for initialization
    void Start ()
    {
        userName = GlobalManager.instance.GetUserName();
        RandomQuizNumber();
    }

    float countTimeLeft = 3;
    bool gameStart;
    bool gameFinish;
    bool gamePause;

    int currentQuestion = 0;
    float timeCountdown = 10;
    int totalScore = 0;
    int totalLives = 2;
    int scoreCorrect = 0;
    int totalCorrected = 0;

    void Update()
    {
        if (!gameStart) {
            if (countTimeLeft > 0)
            {
                countTimeLeft -= Time.deltaTime;
                countdown.text = "Ready in..\n" + Mathf.CeilToInt(countTimeLeft);
            }
            else
            {
                readyPanel.SetActive(false);
                quizPanel.SetActive(true);
                gameStart = true;
                Next();
            }
        }
        else if(!gamePause)
        {
            timeCountdown -= Time.deltaTime;
            timeBar.size -= ((Time.deltaTime) / 10);
            if(timeCountdown <= 0)
            {
                TimesUp();
            }
        }
    }

    Button[] buttonArray;
    public void Next()
    {
        if(currentQuestion == 5 || totalLives == 0)
        {
            EndGame();
            return;
        }

        ResetQuiz();
        Quiz currentQuiz = GlobalManager.instance.GetQuizList().quizList[SelectedQuestions[currentQuestion]];

        question.text = currentQuiz.question;
        roundNumber.text = "Round : " + (currentQuestion + 1);

        buttonArray = new Button[4];
        int number;
        for (int i = 0; i < 4; i++)
        {
            do
            {
                number = Random.Range(0, 4);
            }
            while (buttonArray[number] != null);

            switch (i)
            {
                case 0: buttonArray[number] = choice1; break;
                case 1: buttonArray[number] = choice2; break;
                case 2: buttonArray[number] = choice3; break;
                case 3: buttonArray[number] = choice4; break;
            }
        }

        buttonArray[0].GetComponentInChildren<Text>().text = currentQuiz.corrected;
        buttonArray[1].GetComponentInChildren<Text>().text = currentQuiz.wrong1;
        buttonArray[2].GetComponentInChildren<Text>().text = currentQuiz.wrong2;
        buttonArray[3].GetComponentInChildren<Text>().text = currentQuiz.wrong3;

        Correct(buttonArray[0]);
        Wrong(buttonArray[1]);
        Wrong(buttonArray[2]);
        Wrong(buttonArray[3]);

        currentQuestion++;

    }

    void Correct(Button button)
    {
        button.onClick.AddListener(()=>
        {
            scoreCorrect = Mathf.CeilToInt(100 * timeCountdown);
            totalCorrected++;
            EndOneQuiz();
        });
    }

    void Wrong(Button button)
    {
        button.onClick.AddListener(()=> {
            button.GetComponent<Image>().color = Color.red;
            DecreaseLifePoints();
            EndOneQuiz();
        });
    }

    void TimesUp()
    {
        DecreaseLifePoints();
        EndOneQuiz();
    }

    void ResetQuiz()
    {
        choice1.interactable = true;
        choice2.interactable = true;
        choice3.interactable = true;
        choice4.interactable = true;
        nextButton.interactable = false;

        choice1.onClick.RemoveAllListeners();
        choice2.onClick.RemoveAllListeners();
        choice3.onClick.RemoveAllListeners();
        choice4.onClick.RemoveAllListeners();

        choice1.GetComponent<Image>().color = Color.white;
        choice2.GetComponent<Image>().color = Color.white;
        choice3.GetComponent<Image>().color = Color.white;
        choice4.GetComponent<Image>().color = Color.white;

        nextText.gameObject.SetActive(false);
        scorePlusText.gameObject.SetActive(false);

        scoreCorrect = 0;
        timeCountdown = 10;
        timeBar.size = 1f;
        gamePause = false;

    }

    void EndOneQuiz()
    {
        gamePause = true;

        buttonArray[0].GetComponent<Image>().color = Color.green;
        choice1.interactable = false;
        choice2.interactable = false;
        choice3.interactable = false;
        choice4.interactable = false;
        nextButton.interactable = true;

        nextText.gameObject.SetActive(true);
        scorePlusText.gameObject.SetActive(true);
        scorePlusText.text = "+" + scoreCorrect + " score!";
        totalScore += scoreCorrect;

        if (currentQuestion == 5 || totalLives == 0)
        {
            nextText.text = "End >>";
        }
    }

    void EndGame()
    {
        quizPanel.gameObject.SetActive(false);
        summaryPanel.gameObject.SetActive(true);

        int userHighscore = GlobalManager.instance.GetUserHighScore();
        // If user best hightscore, try updating data
        if (userHighscore < totalScore)
        {
            userHighscore = totalScore;
            GlobalManager.instance.SetUserHighScore(totalScore);
            // Try update highscore to current user score.
            User user = new User(userName, totalScore.ToString());
            string json = JsonUtility.ToJson(user);
            string uniqueId = SystemInfo.deviceUniqueIdentifier;
            GlobalManager.instance.reference.Child("users").Child(uniqueId).ValueChanged += OnUpdateUserScore;
            GlobalManager.instance.reference.Child("users").Child(uniqueId).SetRawJsonValueAsync(json);
            updatingText.text = "Updating, please wait..";
        }
        //If not, just let user continue play the game
        else
        {
            updatingText.text = "";
            EnableSummaryButton();
        }

        highScore.text = "Your Best Score : "+ userHighscore;
        yourScore.text = "Your Game score : "+ totalScore;
        correctedAnswer.text = "Corrected answers : " + totalCorrected;
        heartLeft.text = "Lifepoints left : " + totalLives;
    }

    void EnableSummaryButton()
    {
        retryButton.interactable = true;
        menuButton.interactable = true;
    }

    void DecreaseLifePoints()
    {
        if (totalLives == 2)
            heart2.gameObject.SetActive(false);
        else
            heart1.gameObject.SetActive(false);
        totalLives--;
    }

    void RandomQuizNumber()
    {
        int number;
        for (int i = 0; i < 5; i++)
        {
            do
            {
                number = Random.Range(0, GlobalManager.instance.GetQuizList().quizList.Count);
            }
            while (SelectedQuestions.Contains(number));
            SelectedQuestions.Add(number);
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RetryGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    #region Firbase callbacks
    void OnUpdateUserScore(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Update2");
        if (args.DatabaseError != null)
        {
            updatingText.text = "There is some error, please restart the game.";
        }
        else
        {
            // When finish update score, try get highscore to check if user beat him or not.
            FirebaseDatabase.DefaultInstance.GetReference("highscore").ValueChanged += OnRetrieveHighScore;
        }

    }
    void OnRetrieveHighScore(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Update3");
        if (args.DatabaseError != null)
        {
            updatingText.text = "There is some error, please restart the game.";
        }
        else
        {
            int globalHighScore = int.Parse(args.Snapshot.Value.ToString());
            // If user beat global highscore, try updating this..***NEED OPTIMIZATION!!
            if (globalHighScore < totalScore)
            {
                GlobalManager.instance.reference.Child("highscore").ValueChanged += OnUpdateHighScore;
                GlobalManager.instance.reference.Child("highscore").SetValueAsync(totalScore);
            }
            //If not, just let user know he beat his own best, and let user continue the game
            else
            {
                updatingText.text = "Congrats, " + userName + ".\nYou just beat your best score!";
                EnableSummaryButton();
            }
        }
    }

    void OnUpdateHighScore(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Update4");
        if (args.DatabaseError != null)
        {
            updatingText.text = "There is some error, please restart the game.";
        }
        else
        {
            // Finish update score and let user contine the game
            updatingText.text = "PERFECT, " + userName + "!!.\nYou just beat the global best score!!!";
            EnableSummaryButton();
        }
    }
    #endregion
}
