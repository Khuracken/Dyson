using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PScore : MonoBehaviour
{
    public Text textScore;
    public float savedScore;
    public float scoreIncreasePerSec;

    public void Start()
    {
        savedScore = 0;
        textScore.text = "0";
        scoreIncreasePerSec = 0.5f;
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Players").Length == 0 )
        {
            SceneManager.LoadScene("Menu");
        }

        savedScore += scoreIncreasePerSec * Time.deltaTime;
        SetScore();
    }

    public void SetScore()
    {
        //savedScore = savedScore + score;
        Debug.Log(savedScore);
        textScore.text = Mathf.Round(savedScore).ToString();
    }
}
