using UnityEngine;

public class AudioPlayer : MonoBehaviour
{   

    public static AudioPlayer Instance { get; private set; }
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip fail;
    [SerializeField] private AudioClip lit;
    [SerializeField] private float pitch;
    [SerializeField] private float initialPitch;

    private void Awake(){
        Instance = this;

        sfxSource.pitch = initialPitch;
        audioSource.loop = true;

    }

    public void PlayMusic(LevelData levelData){
        audioSource.clip = levelData.backgroundMusic;
        audioSource.Play();
    }

    public void failSFX(){
        sfxSource.pitch = 1f; ;
        sfxSource.PlayOneShot(fail);
    }

    public void litSFX(){
        sfxSource.pitch += pitch;
        sfxSource.PlayOneShot(lit);
    }

    public void resetSFX(){
        sfxSource.pitch = initialPitch;
    }
}   

