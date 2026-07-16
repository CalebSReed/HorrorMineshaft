using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play("Ambience1", transform.position, null, true, false, true);
        AudioManager.Instance.Play("Ambience2", transform.position, null, true, false, true);
    }
}
