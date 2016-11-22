using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Unity.Editor;
using System.Collections.Generic;
using System;
using Firebase.Database;

public class GlobalManager : MonoBehaviour
{

    public static GlobalManager instance;
    public DatabaseReference reference;

    // Use this for initialization
    void Awake ()
	{
        instance = this;
        DontDestroyOnLoad(this);
		// Set these values before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ("https://ezquiz-155cc.firebaseio.com/");
		FirebaseApp.DefaultInstance.SetEditorP12FileName ("EzQuiz-9b198753b3a0.p12");
		FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail ("ezquiz-155cc@appspot.gserviceaccount.com");
		FirebaseApp.DefaultInstance.SetEditorP12Password ("notasecret");

        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        // HACK, I don't why, both these objects were destroy on loaded and it make callback never return,
        // So I decided to hack by find these both objects and save them, and callback is works like a charm!! 
        DontDestroyOnLoad(GameObject.Find("Firebase Services"));
        DontDestroyOnLoad(GameObject.Find("FirebaseSynchronizationContext"));
    }

    private int globalScore = 0;
    private int userScore = 0;
    private string userName = "";
    private QuizList QuizesList;

    public int GetGlobalHighScore()
    {
        return globalScore;
    }

    public void SetGlobalHighScore(int score)
    {
        globalScore = score;
    }

    public void SetUserName(string name)
    {
        userName = name;
    }

    public string GetUserName()
    {
        return userName;
    }

    public int GetUserHighScore()
    {
        return userScore;
    }

    public void SetUserHighScore(int score)
    {
        userScore = score;
    }

    public void SetQuizList(QuizList list)
    {
        QuizesList = list;
    }

    public QuizList GetQuizList()
    {
        return QuizesList;
    }

}
