using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingSequence : MonoBehaviour
{
    [SerializeField] Slider ProgressSlider;
    [SerializeField] GameObject loadingScreen;
    public void LoadScene(int lvlIndex)
    {
        StartCoroutine(LoadingSceneAsync(lvlIndex)); // 
    }
    
    IEnumerator LoadingSceneAsync(int lvlIndex) // Corutine Method to call a method 
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(lvlIndex); // Load the Game's Level while other things still continue in the background
        loadingScreen.SetActive(true);
        while(asyncOperation.isDone == false) // If the Progression isn't done yet then...
        {
            ProgressSlider.value = asyncOperation.progress; // Updates the progress bar, until this is done then it wait and then return null which then exits this while loop 
            yield return null; // Waits for the next frame
        }
    }
}
