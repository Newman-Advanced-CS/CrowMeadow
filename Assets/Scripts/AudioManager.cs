using System.Collections;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string ID;
    public AudioClip sound;
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void Play(string ID, float volume, float pitch, bool loop)
    {
        // Find the correct sound
        bool foundSound = false;
        Sound toPlay = new Sound();
        for(int i = 0; i < instance.sounds.Length && !foundSound; i++)
        {
            Sound currentSound = instance.sounds[i];
            if(currentSound.ID == ID)
            {
                toPlay = currentSound;
                foundSound = true;
            }
        }

        // Play the sound
        AudioSource source = instance.gameObject.AddComponent<AudioSource>();
        source.clip = toPlay.sound;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.Play();

        if(!loop)
            instance.StartCoroutine(DespawnSource(source.clip.length, source));

    }

    public static IEnumerator DespawnSource(float clipLength, AudioSource source)
    {
        yield return new WaitForSeconds(clipLength);
        Destroy(source);
    }
}
