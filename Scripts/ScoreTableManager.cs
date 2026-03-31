using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreTableManager : MonoBehaviour
{
    LevelManager levelManager;

    public TMP_Text finalScoreTxt;
    public TMP_Text _GameOverScreenFinalScoreTxt;
    public TMP_Text _LoseScreenFinalScoreTxt;
    public TMP_Text highScoreTxt;
    public TMP_Text _GameOverScreenHighScoreTxt;
    public TMP_Text _LoseScreenHighScoreTxt;
    public TMP_Text delMessage;
    public GameObject _delMessageGameObject;
   
    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        
    }

    public void HighScoreTableDataUpdate() // Invoke this method at the end of the game when the player dies, on the Game Over UI menu, display the highscore
    {
        if(PlayerPrefs.HasKey("SavedHighScore")) // If there is a Highscore
        {
            if(levelManager.score > PlayerPrefs.GetInt("SavedHighScore")) // And if the current score is more than the current Saved high score
            {
                PlayerPrefs.SetInt("SavedHighScore", levelManager.score); // Set the current highscore as the latest High score
            }
        }
        else
        {
            PlayerPrefs.SetInt("SavedHighScore", levelManager.score); // If there is no Saved High score than set the current high score as the Saved High score 
        }

        // Player Final Score Ui Text
        finalScoreTxt.text = "Your Score: " + levelManager.score.ToString(); // Sets the final score as the current score
        _GameOverScreenFinalScoreTxt.text = "Your Score: " + levelManager.score.ToString();
        _LoseScreenFinalScoreTxt.text = "Your Score: " + levelManager.score.ToString();

        // Highscore Ui Text
        highScoreTxt.text = "Highscore: " + PlayerPrefs.GetInt("SavedHighScore").ToString(); // Sets the high score to UI text. 
        _GameOverScreenHighScoreTxt.text = "Highscore: " + PlayerPrefs.GetInt("SavedHighScore").ToString();
        _LoseScreenHighScoreTxt.text = "Highscore: " + PlayerPrefs.GetInt("SavedHighScore").ToString();
    }

    public void DisplayCurrentHighScoreData() // Maybe invoke this method when the player presses Tab
    {
        if(PlayerPrefs.HasKey("SavedHighScore")) // If there's a Highscore
        {
            finalScoreTxt.text = "Your Score: " + levelManager.score.ToString(); // Sets the final score as the current score
            highScoreTxt.text = "Highscore: " + PlayerPrefs.GetInt("SavedHighScore").ToString(); // Sets whatever highscore thats been saved to playerprefs 
        }
        else
        {
            finalScoreTxt.text = "Your Score: " + levelManager.score.ToString(); // Sets the final score as the current score
            highScoreTxt.text = "There is No Highscore";
        }
    }

    public void DeleteSavedHighScoreData() // Invoke this Method when the delete button is pressed or by pressing the debug key it would reset the highscore 
    {
       
        if(PlayerPrefs.HasKey("SavedHighScore")) // if there is a SavedHighScore
        {
            PlayerPrefs.DeleteKey("SavedHighScore"); // Delete the Saved data in the player prefs
            delMessage.text = "All Highscore data has been deleted"; // Message that confirms that the HighScore has been deleted 
        }
        else
        {
            delMessage.text = "There are no saved highscore data currently"; // If there is no Saved High score display this message 
            //Debug.Log("There is currently no saved Highscore");
        }
    }
}
