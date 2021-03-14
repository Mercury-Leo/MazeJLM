using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicControl : MonoBehaviour
{
    [SerializeField] Sprite musicOnImage;
    [SerializeField] Sprite musicOffImage;
    [SerializeField] Button musicBtn;
    [SerializeField] GameObject musicObject;

    private GameObject audioaObject;
    private AudioSource audioSource;

    private void Awake()
    {
        
    }

    void Start()
    {
        audioaObject = Instantiate(musicObject, transform);
        audioSource = audioaObject.transform.GetComponent<AudioSource>();
    }

    public void ControlBGMusic()
    {

        if (musicBtn.image.sprite == musicOnImage)
        {
            musicBtn.image.sprite = musicOffImage;
            audioSource.Pause();
            //audioaObject.SetActive(false);
        }
        else
        {
            musicBtn.image.sprite = musicOnImage;
            audioSource.Play();
            //audioaObject.SetActive(true);
        }

    }
}
