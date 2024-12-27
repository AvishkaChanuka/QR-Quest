using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip backgroundMusic;  // Drag your background music here in the inspector
    private AudioSource audioSource;

    void Awake()
    {
        // Check if there are other instances of the AudioManager and destroy them
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);  // Destroy this instance
            return;
        }

        // Make this object persist across scenes
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;  // Loop the music
        audioSource.Play();  // Start playing the music
    }
}
