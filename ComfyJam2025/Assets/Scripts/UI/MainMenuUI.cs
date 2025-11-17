using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject initialPanel;

    [Header("SLIDESHOW CUTSCENE")]
    public List<Sprite> cutsceneImages;
    [SerializeField] private Image cutsceneImage;

    [Header("CUTSCENE SETTINGS")]
    [Tooltip("seconds per frame for 1st auto-play section (6-11, aka where potion falls)")]
    [SerializeField] private float normalFrameSpeed = 2f;
    [Tooltip("seconds per frame for the 2nd auto-play section (19-24, aka the end scene)")]
    [SerializeField] private float slowFrameSpeed = 3.5f;

    

    [Header("UI REFERENCES")]
    [SerializeField] private TextMeshProUGUI nextButtonText;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private bool hideButtonsOnClamp = true;
    [SerializeField] private bool switchScenesOnClamp = true;

    private GameObject currentPanel;
    private int currentCutsceneIndex = 0;
    private bool isAutoplaying = false; 


    private void Start() {
        currentPanel = initialPanel;
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // Tell AudioManager to play calm theme
        AudioManager.instance.PlayMusic(MusicTrack.CalmTheme);
        }
        
    }

    public void OpenPanel(GameObject goToPanel) {
        currentPanel.SetActive(false);
        currentPanel = goToPanel;
        currentPanel.SetActive(true);
    }
    public void StartCutscene(GameObject goToPanel) {
        OpenPanel(goToPanel);

        currentCutsceneIndex = 0;
        DisplayImage(currentCutsceneIndex);
    }

    
    public void ShowNextImage() {
        // disable next when autoplaying
        if (isAutoplaying) return;

        if (currentCutsceneIndex == 14)
        {
            // Tell AudioManager to play calm theme
            AudioManager.instance.PlayMusic(MusicTrack.DramaticTheme);         
        }

        // start autoplay if 6 ( 6- 11)
        if (currentCutsceneIndex == 5) {
            AudioManager.instance.PlayCutscenePotionHit();
            StartCoroutine(PlayAnimSequence(6, 11, normalFrameSpeed));
            return; // let coroutine autoplay helper do its thing
        }
        
        // start autoplay if 18 ( 19 - 24)
        if (currentCutsceneIndex == 18) {
            // (Uses cutsceneImages.Count-1 to be safe)
            StartCoroutine(PlayAnimSequence(19, 24, slowFrameSpeed));
            return; // let coroutine autoplay helper do its thing
        }

        // IF END -> start game
        if (currentCutsceneIndex >= cutsceneImages.Count - 1) {
            if (switchScenesOnClamp) {
                AudioManager.instance.PlayMusic(MusicTrack.None);
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
                gameObject.SetActive(false);
            }
            return; // FINAL DONE
        }

        // MANUAL CLICKY FORWARD if got here (neither the start of autoplay, nor the end)
        currentCutsceneIndex++;
        DisplayImage(currentCutsceneIndex);
    }

    public void ShowPrevImage() {

        if (isAutoplaying) return; // prevent prev button while autoplaying

        currentCutsceneIndex--;

        if (currentCutsceneIndex >= 19) {
            // for 19-24 auto-play zone, go back to before autoplay section (18)
            currentCutsceneIndex = 18;
        }
        else if (currentCutsceneIndex >= 6 && currentCutsceneIndex <= 11) {
            // for 6-11 auto-play zone, go back to before autoplay section (6)
            currentCutsceneIndex = 5;
        }
        else {
            // DEAFULT: back 1 frame
            currentCutsceneIndex--;
        }

        DisplayImage(currentCutsceneIndex);
    }

    private void DisplayImage(int index) {
        currentCutsceneIndex = Mathf.Clamp(index, 0, cutsceneImages.Count - 1);  // clamping in case

        if (cutsceneImages.Count > 0) {
            cutsceneImage.sprite = cutsceneImages[currentCutsceneIndex];
        }

        if (!hideButtonsOnClamp) return;

        if (currentCutsceneIndex == 0) {
            prevButton.SetActive(false);
        }

        else {
            prevButton.SetActive(true);
        }

        nextButton.SetActive(true);

        if (nextButtonText == null) return;

        if (currentCutsceneIndex == cutsceneImages.Count - 1) {

            nextButtonText.text = "Start Game";
        }
        else {

            nextButtonText.text = ">>";
        }
    }


    // handles auto-play sections

    private IEnumerator PlayAnimSequence(int startFrame, int endFrame, float speed)

    {
        isAutoplaying = true;
        nextButton.SetActive(false);
        // prevButton.SetActive(false);

        // iterate along specified frames (autoplay)
        for (int i = startFrame; i <= endFrame; i++)
        {
            // check valid  indx
            if (i >= cutsceneImages.Count) break; 

            currentCutsceneIndex = i;
            cutsceneImage.sprite = cutsceneImages[currentCutsceneIndex];
            yield return new WaitForSeconds(speed);
        }

        // animation done
        isAutoplaying = false;

        // call helper to update buttons correctly for this last frame
        DisplayImage(currentCutsceneIndex);
    }

}
