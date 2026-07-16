using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public void PlayStepSound(AnimationEvent anim)
    {
        var rand = Random.Range(1, anim.intParameter+1);//if u select 3 then poss outputs are 1, 2, 3
        AudioManager.Instance.Play($"{anim.stringParameter}{rand}", transform.position, gameObject, false, false, false);
    }
}
