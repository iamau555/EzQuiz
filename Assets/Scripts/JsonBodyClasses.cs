using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
class User
{
    public string name;
    public string score;

    public User(string username, string score = "0")
    {
        this.name = username;
        this.score = score;
    }
}

[Serializable]
public class Quiz
{
    public string question;
    public string corrected;
    public string wrong1;
    public string wrong2;
    public string wrong3;
}

[Serializable]
public class QuizList
{
    public List<Quiz> quizList;
}