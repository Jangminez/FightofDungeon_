using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip death;
    public AudioClip attack;
    public AudioClip hit;

    [Header("Player Skills")]
    public AudioClip skill1;
    public AudioClip skill2;
    public AudioClip skill3;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDeathSFX()
    {
        if (death != null)
            audioSource.PlayOneShot(death);
    }

    public void PlayAttackSFX()
    {
        if (attack != null)
            audioSource.PlayOneShot(attack);
    }

    public void PlayHitSFX()
    {
        if (hit != null)
            audioSource.PlayOneShot(hit);
    }

    public void PlaySkill1SFX()
    {
        if (skill1 != null)
            audioSource.PlayOneShot(skill1);
    }

    public void PlaySkill2SFX()
    {
        if (skill2 != null)
            audioSource.PlayOneShot(skill2);
    }

    public void PlaySkill3SFX()
    {
        if (skill3 != null)
            audioSource.PlayOneShot(skill3);
    }
}
