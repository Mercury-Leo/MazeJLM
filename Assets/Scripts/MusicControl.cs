using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicControl : MonoBehaviour
{
    static MusicControl Instance;

    [SerializeField] Sprite musicOnImage;
    [SerializeField] Sprite musicOffImage;
    [SerializeField] Button musicBtn;
    [SerializeField] GameObject musicObject;

    private GameObject audioaObject;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null & Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
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
