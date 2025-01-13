using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedMomentarySoundEffect : MonoBehaviour
{
    public AudioSource snd;
    static string prefabPath = "Scavenger/prefab/SpawnedSoundEffect";
    public static void SpawnSnd(Vector3 location, AudioClip clip = null)
    {
        //spawn sound effect
        GameObject gameObject = Resources.Load<GameObject>(prefabPath);
        GameObject newObj = Instantiate(gameObject, location, Quaternion.identity);
        float rand = Random.Range(0.0f, 2.0f);
        SpawnedMomentarySoundEffect script = newObj.GetComponent<SpawnedMomentarySoundEffect>();
        script.snd.pitch = rand;
        //Todo: add randomness
        //Todo: switch audioclip
        if(clip != null) { script.snd.clip = clip; }
        script.snd.Play();

    }
}
