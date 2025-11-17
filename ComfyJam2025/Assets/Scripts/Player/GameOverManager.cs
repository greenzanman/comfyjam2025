using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("game over UI")]
    public GameObject gameOverCanvas;  // parent
    public Image flashImage;

    [Header("game over fx settings")]
    public float sequenceDuration = 4.0f;
    public float zoomTargetSize = 3.0f;

    private void Awake()
    {
        // one instance only
        if (instance == null) instance = this;
        
        // make sure shit is disabled at the start
        if(gameOverCanvas) gameOverCanvas.SetActive(false);
        if(flashImage) 
        {
            Color c = flashImage.color;
            c.a = 0;
            flashImage.color = c;
        }
    }

    // for other ppl to call
    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // 1. Set Game State
        GameManager.isGameOver = true; 

        // fx - shake and flash
        CameraShake.Instance.Shake(1.0f, 1.0f); 
        StartCoroutine(FlashCameraFX());

        // sfx
        AudioManager.instance.PlaySpellSound(SpellSound.FireBoom);
        AudioManager.instance.PlayMusic(MusicTrack.None);

        // fx - zoom
        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
        float timer = 0;

        // slowmo
        while (timer < sequenceDuration)
        {
            timer += Time.unscaledDeltaTime;  // unscaled so zoom happens independent of slowmo fx
            float linearT = Mathf.Clamp01(timer / sequenceDuration);
            float easeT = Mathf.Sin(linearT * Mathf.PI * 0.5f);

            // zoom camera over the time
            cam.orthographicSize = Mathf.Lerp(startSize, zoomTargetSize, easeT);

            yield return null;
        }

        // show game over UI
        gameOverCanvas.SetActive(true);
        AudioManager.instance.PlayMusic(MusicTrack.GameOver);
    }

    private IEnumerator FlashCameraFX()
    {
        // flash instant white
        Color c = flashImage.color;
        c.a = 0.8f;
        flashImage.color = c;

        // 1s fade out
        float duration = 1.0f;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0.8f, 0f, timer / duration);
            flashImage.color = c;
            yield return null;
        }
    }

    // restarts the game scene
    public void RestartFromGameover()
    {
        // reset game over flag
        GameManager.isGameOver = false; 
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}