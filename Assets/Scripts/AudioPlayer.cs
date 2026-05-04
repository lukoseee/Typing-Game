using UnityEngine;

public class AudioPlayer : MonoBehaviour
{   

    public static AudioPlayer Instance { get; private set; }
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource; //for lit sfx
    [SerializeField] private AudioSource failSFXSource;

    [SerializeField] private AudioClip fail;
    [SerializeField] private AudioClip lit;

    [SerializeField] private float pitch;
    [SerializeField] private float initialPitch;

    private void Awake(){
        Instance = this;

        sfxSource.pitch = initialPitch;
        audioSource.loop = true;
    }

    //bg music
    public void PlayMusic(LevelData levelData){
        audioSource.clip = levelData.backgroundMusic;
        audioSource.Play();
    }

    //play fail
    public void failSFX(){
        failSFXSource.pitch = 1f; 
        failSFXSource.PlayOneShot(fail);
    }

    //play lit
    public void litSFX(){
        //increase pitch for each lit candle
        sfxSource.pitch += pitch;
        sfxSource.PlayOneShot(lit);
    }

    //reset lit sfx pitch
    public void resetSFX(){
        sfxSource.pitch = initialPitch;
    }
}   

