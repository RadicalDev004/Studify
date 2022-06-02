using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
public class VideoManager : MonoBehaviour
{
    private VideoPlayer Video;
    public Slider Progress;
    public TMP_Text CurrentTime, FullTime;
    public GameObject PlayStatus;
    public float SavedPos;
    public bool IsPressingSlider = false;
    public bool IsPlaying = false;
    
    void Start()
    {
        Video = GetComponent<VideoPlayer>();
        FullTime.text = FormatTime((float)Video.clip.length);

        Progress.maxValue = Video.frameCount;
    }

    void Update()
    {
        if(!IsPressingSlider) {
            Progress.value = Video.frame;
            CurrentTime.text = FormatTime((float)Video.time);
        }
          
        PlayStatus.SetActive(!IsPlaying);
    }

    public void PausePlay()
    {
        if (Video.isPlaying) {
            Video.Pause();
            IsPlaying = false;
        }
        else if (!Video.isPlaying) {
            Video.Play();
            IsPlaying = true;
        }
    }

    public void OnSliderValueChanged()
    {
        CurrentTime.text = FormatTime(Progress.value / Video.frameRate);
    }

    public void OnSliderPress()
    {
        IsPressingSlider = true;
        Video.Pause();
    }

    public void OnSliderRelease()
    {
        Video.frame = (long)Progress.value;
        if(IsPlaying) Video.Play();
        Invoke(nameof(StopPressingSlider), 0.25f);
    }
    public void StopPressingSlider() => IsPressingSlider = false;

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
