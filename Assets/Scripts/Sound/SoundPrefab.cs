using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPrefab : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    public string soundName;
    public Sound.SoundType soundType;
    public AudioClip clip;
    public float volMult;
    public bool loops;
    public int progress;
    public float goal;
    public bool follow;
    public GameObject source;
    public float noiseRadius;
    public float noiseVal;
    private Coroutine countdown;

    public void StartTimer()
    {
        if (clip != null && !loops)
        {
            progress = 0;
            goal = clip.length;
            countdown = StartCoroutine(DisableOnSoundEnd());

            var allListeners = Physics.OverlapSphere(transform.position, noiseRadius);

            foreach (var listener in allListeners)
            {
                if (listener.attachedRigidbody && listener.attachedRigidbody.GetComponent<HearingManager>() != null && Vector3.Distance(transform.position, listener.transform.position) <= noiseRadius)
                {
                    listener.attachedRigidbody.GetComponent<HearingManager>().HearSound(transform.position, RecalculateNoiseValue(listener.transform));
                    return;
                }
            }
        }
    }

    private float RecalculateNoiseValue(Transform listener)//reduces noise value linearly the farther the listener is.
    {
        float newNoiseVal = (noiseRadius - Vector3.Distance(transform.position, listener.position)) / (noiseRadius / noiseVal);
        Debug.Log($"new val is {newNoiseVal}, dist: {Vector3.Distance(transform.position, listener.position)}");
        return newNoiseVal;
    }

    public void ResumeTimer()
    {
        if (countdown == null && !loops)
        {
            countdown = StartCoroutine(DisableOnSoundEnd());
        }
    }

    public void PauseTimer()
    {
        StopAllCoroutines();
        countdown = null;
    }

    private void Update()
    {
        if (follow)
        {
            if (source != null)
            {
                transform.position = source.transform.position;
            }
            else
            {
                //mute sound
            }            
        }
    }

    public IEnumerator DisableOnSoundEnd()
    {
        yield return new WaitForSeconds(1);
        progress++;
        if (progress > goal)
        {
            AudioManager.Instance.DestroySound(gameObject);
            countdown = null;
            follow = false;
            soundName = "";
            yield break;
        }
        StartCoroutine(DisableOnSoundEnd());
    }
}
