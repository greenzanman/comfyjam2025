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

    [SerializeField] private TextMeshProUGUI nextButtonText;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;

    private GameObject currentPanel;
    private int currentCutsceneIndex = -1;

    private void Start() {
        currentPanel = initialPanel;
    }

    public void OpenPanel(GameObject goToPanel) {
        currentPanel.SetActive(false);
        currentPanel = goToPanel;
        currentPanel.SetActive(true);
    }
    public void StartCutscene(GameObject goToPanel) {
        OpenPanel(goToPanel);

        currentCutsceneIndex = -1;
        ShowNextImage();
    }
    public void ShowNextImage() {
        currentCutsceneIndex++;

        if (currentCutsceneIndex > cutsceneImages.Count - 1) {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }

        currentCutsceneIndex = Mathf.Clamp(currentCutsceneIndex, 0, cutsceneImages.Count-1);
        cutsceneImage.sprite = cutsceneImages[currentCutsceneIndex];
        
        if (currentCutsceneIndex == 0) {
            prevButton.SetActive(false);
        }
        else {
            prevButton.SetActive(true);
        }

        if (currentCutsceneIndex == cutsceneImages.Count - 1) {
            nextButtonText.text = "Start Game";
        }        
        else {
            nextButtonText.text = ">>";
        }
    }
    public void ShowPrevImage() {
        currentCutsceneIndex--;
        currentCutsceneIndex = Mathf.Clamp(currentCutsceneIndex, 0, cutsceneImages.Count-1);
        cutsceneImage.sprite = cutsceneImages[currentCutsceneIndex];

        if (currentCutsceneIndex == 0) {
            prevButton.SetActive(false);
        }
        else {
            prevButton.SetActive(true);
        }

        if (currentCutsceneIndex == cutsceneImages.Count - 1) {
            nextButtonText.text = "Start Game";
        }
        else {
            nextButtonText.text = ">>";
        }
    }
}
