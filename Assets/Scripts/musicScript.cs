using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicScript : MonoBehaviour
{
    public AudioSource audioSource;
    
    public AudioClip backgroundClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    private RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        rubyController = rubyControllerObject.GetComponent<RubyController>();

        audioSource.clip = backgroundClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (rubyController.currentHealth == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(loseClip);
            Destroy(this);
        }

        if (rubyController.score == 4)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(winClip);
            Destroy(this);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        rubyController.score = rubyController.score +1;
    }
}
