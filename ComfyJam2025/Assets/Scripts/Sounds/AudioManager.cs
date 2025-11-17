using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// so apparentyly PlayOneShot (diff than Play) allows for playing multiple sounds of same audiosource, 

public enum MusicTrack
{
    None,
    CalmTheme,
    DramaticTheme,
    MediumBattle,
    IntenseBattle,
}

public enum SpellSound
{
    FreezeStart,
    FreezeEnd,
    Zappy,
    Sunbeam,
    FireWall,
    FireBoom,
    Summon,
    FreezeFlare
}

public class AudioManager : MonoBehaviour
{
    [Header("Gameplay Music")]
    private AudioSource _currentMusicTrack;
    [SerializeField] private AudioSource _mediumBattleMusic;
    [SerializeField] private AudioSource _intenseBattleMusic;
    [SerializeField] private float _musicFadeDuration = 1.0f;

    [Header("Other Music")]
    [SerializeField] private AudioSource _calmThemeMusic;
    
    [SerializeField] private AudioSource _dramaticThemeMusic;


    // setup constants for muffled/lowpass filter to work:
    private const float LOW_PASS_CUTOFF = 1500f;  // muffled sound 
    private const float NORMAL_CUTOFF = 22000f; // normal sound (lets all frequencies through)


    [Header("Spell SFX")]
    [SerializeField] private AudioSource _freezeStartSource;
    [SerializeField] private AudioSource _freezeEndSource;
    [SerializeField] private AudioSource _zappySource;
    [SerializeField] private AudioSource _sunbeamSource;
    [SerializeField] private AudioSource _fireWallSource;
    [SerializeField] private AudioSource _fireBoomSource;
    [SerializeField] private AudioSource _summonSource;
    [SerializeField] private AudioSource _freezeFlareSource;


    [Header("Enemy SFX")]
    [SerializeField] private AudioSource _enemySFXPlayer;
    [SerializeField] private AudioClip[] _enemyDeathClips;
    [SerializeField] private AudioClip[] _enemyHitClips;  
    [SerializeField] private AudioSource _enemyBurnLoopSource;
    [SerializeField] private AudioSource _enemyFrozenLoopSource;

    [Header("Enemy Sound Limiting")]
    [SerializeField] private int _maxSimultaneousDeathSounds = 5;
    private int _currentDeathSoundsPlaying = 0;

    [SerializeField] private float _hitSoundCooldown = 0.3f; 
    private float _lastHitSoundTime;


    [Header("Meow SFX")]
    [SerializeField] private AudioSource _meowSource;

    [Header("UI SFX")]
    [SerializeField] private AudioSource _uiBlinkSource;
    [SerializeField] private AudioSource _uiHoverSource;
    [SerializeField] private AudioSource _hitpotionSFX;
    [SerializeField] private AudioSource _slot1Source;
    [SerializeField] private AudioSource _slot2Source;
    [SerializeField] private AudioSource _slot3Source;

    

    
    public static AudioManager instance;

    // the single coroutine for the muffled filter transitions
    private Coroutine _filterCoroutine;


    // persist this single AudioManager across scene loads
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Play music track, and fade out the old one. 
    ///  - pass in a MusicTrack enum like PlayMusic(MusicTrack.MediumBattle);
    /// </summary>
    /// <param name="muffleOn">True to toggle on the muffled filter, False to remove it.</param>
    public void PlayMusic(MusicTrack musicTrack, float targetVolume = 0.5f)
    {
        AudioSource trackToPlay = null;
        switch (musicTrack)
        {
            case MusicTrack.None:
                break;
            case MusicTrack.CalmTheme:
                trackToPlay = _calmThemeMusic;
                break;
            case MusicTrack.DramaticTheme:
                trackToPlay = _dramaticThemeMusic;
                break;
            case MusicTrack.MediumBattle:
                trackToPlay = _mediumBattleMusic;
                break;
            case MusicTrack.IntenseBattle:
                trackToPlay = _intenseBattleMusic;
                break;
        }

        // fade out old track
        if (_currentMusicTrack  != null && _currentMusicTrack != trackToPlay)
        {
            StartCoroutine(FadeTrack(_currentMusicTrack, false));
        }

        // play new track
        _currentMusicTrack = trackToPlay;
        if (_currentMusicTrack != null && !_currentMusicTrack.isPlaying)
        {
            _currentMusicTrack.volume = 0.0f;
            StartCoroutine(FadeTrack(_currentMusicTrack, true, targetVolume));  // fade in
        }

    }

    // Helper method to fade in or out a music track
    private IEnumerator FadeTrack(AudioSource track, bool fadeIn, float targetVolume = 0.5f)
    {
        float time = 0;
        float startVolume = track.volume;
        float finalTargetVolume = fadeIn ? targetVolume : 0;

        // for fade in tracks: start playing before fade transition
        if (fadeIn)
        {
            track.Play();
        }

        while (time < _musicFadeDuration)
        {
            // transition volume to the final target volume
            track.volume = Mathf.Lerp(startVolume, finalTargetVolume, time / _musicFadeDuration);
            
            time += Time.deltaTime;
            yield return null; // apparently needed to make while loop do 1 iteration per frame
        }

        track.volume = finalTargetVolume;


        // for fade-out tracks: stop playing after transition
        if (!fadeIn)
        {
            track.Stop();
            track.volume = startVolume; // Reset volume for next time
        }
    }


    /// <summary>
    /// Applies or removes a muffled/lowpass filter on current music track.
    /// </summary>
    /// <param name="muffleOn">True to toggle on the muffled filter, False to remove it.</param>
    public void ToggleMusicMuffled(bool muffleOn)
    {
        // don't do if no music is even playing
        if (_currentMusicTrack == null)
        {
            return;
        }

        // get the filter component on the music
        AudioLowPassFilter filter = _currentMusicTrack.GetComponent<AudioLowPassFilter>();

        // debug warniug if filter component not found
        if (filter == null) {
            Debug.LogWarning("AudioManager WWARNING AAHHH: No 'AudioLowPassFilter' component found for: " + _currentMusicTrack.name);
            return;
        }

        // stop any currently running filter trasnition
        if (_filterCoroutine != null)
        {
            StopCoroutine(_filterCoroutine);
        }

        // 

        // apply/remove the filter based on muffleOn arg
        float targetFrequency = muffleOn ? LOW_PASS_CUTOFF : NORMAL_CUTOFF;
        _filterCoroutine = StartCoroutine(SmoothMuffleTransition(filter, targetFrequency, 0.5f));
    }
    // Helper to make muffle transition smooth
    private IEnumerator SmoothMuffleTransition(AudioLowPassFilter filter, float targetFrequency, float duration)
    {

        float currentTime = 0;
        float startFrequency = filter.cutoffFrequency;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime; // unscaled so still works when slowmo
            float newFreq = Mathf.Lerp(startFrequency, targetFrequency, currentTime / duration);  // transition with linear interpolation
            filter.cutoffFrequency = newFreq;
            yield return null;
        }

        filter.cutoffFrequency = targetFrequency;  // should already reach targetfreq, but just in case.
        _filterCoroutine = null;
    }



    // SFX 🎶🎶🎶🎶🎶🎵🎵🎶🎶🎵🎵
    // i referenced code from :https://medium.com/@cwagoner78/creating-an-organized-sound-system-and-playing-sounds-in-unity-82cbb48060ff
    // - does pitch randomization for randomdly selected sound from array of sounds


    // PLAY ENEMY HIT SOUND
    public void PlayEnemyHit()
    {
        if (Time.time > _lastHitSoundTime + _hitSoundCooldown)
        {
            StartCoroutine(PlayEnemyHitCoroutine());
            _lastHitSoundTime = Time.time;
        }
    }
    // - with slight random delay
    // - with slight random pitch variation
    private IEnumerator PlayEnemyHitCoroutine()
    {
        float delay = Random.Range(0f, 0.05f);
        yield return new WaitForSeconds(delay);

        AudioClip clip = _enemyHitClips[Random.Range(0, _enemyHitClips.Length)];
        _enemySFXPlayer.pitch = Random.Range(0.9f, 1.1f);
        _enemySFXPlayer.PlayOneShot(clip);
    }




    // PLAY ENEMY DEATH SOUND
    public void PlayEnemyDeath()
    {
        if (_currentDeathSoundsPlaying >= _maxSimultaneousDeathSounds)
        {
            return;
        }

        StartCoroutine(PlayEnemyDeathCoroutine());
    }
    // - with slight random delay
    // - with slight random pitch variation
    private IEnumerator PlayEnemyDeathCoroutine()
    {
        _currentDeathSoundsPlaying++;

        AudioClip clip = _enemyDeathClips[Random.Range(0, _enemyDeathClips.Length)];
        float delay = Random.Range(0f, 0.2f);

        yield return new WaitForSeconds(delay);  // slight startup random delay
        
        _enemySFXPlayer.pitch = Random.Range(0.9f, 1.1f);
        _enemySFXPlayer.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);
        _currentDeathSoundsPlaying--;

    }


    public void PlaySpellSound(SpellSound sound)
    {
        switch (sound)
        {
            case SpellSound.FreezeStart:
                _freezeStartSource.Play();
                break;
            case SpellSound.FreezeEnd:
                _freezeEndSource.Play();
                break;
            case SpellSound.Zappy:
                _zappySource.Play();
                break;
            case SpellSound.Sunbeam:
                _sunbeamSource.Play();
                break;
            case SpellSound.FireWall:
                _fireWallSource.Play();
                break;
            case SpellSound.FireBoom:
                _fireBoomSource.Play();
                break;
            case SpellSound.Summon:
                _summonSource.Play();
                break;
            case SpellSound.FreezeFlare:
                _freezeFlareSource.Play();
                break;
        }
    }



    // UI SOUNDS
    public void PlayUIBlink()
    {
        _uiBlinkSource.Play();
    }

    public void PlayUIHover()
    {
        _uiHoverSource.Play();
    }

    public void PlayMeow()
    {
        _meowSource.pitch = Random.Range(0.9f, 1.1f);
        _meowSource.Play();
    }

    public void PlayCutscenePotionHit()
    {
        _hitpotionSFX.Play();
    }

    public void PlayCraftingSlotSound(int ingredientCount)
    {
        switch (ingredientCount)
        {
            case 1:
                _slot1Source.Play();
                break;
            case 2:
                _slot2Source.Play();
                break;
            case 3:
                _slot3Source.Play();
                break;
        }
    }
}