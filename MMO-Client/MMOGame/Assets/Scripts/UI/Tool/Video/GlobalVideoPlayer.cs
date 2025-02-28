using HSFramework.MySingleton;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public class GlobalVideoPlayer : Singleton<GlobalVideoPlayer>
{
    private VideoPlayer videoPlayer;
    private Action playbackCompletedAction;
    private RawImage rawImage;

    protected override void Awake()
    {
        base.Awake();
        videoPlayer = transform.Find("RawImage").GetComponent<VideoPlayer>();
        rawImage = transform.Find("RawImage").GetComponent<RawImage>();
    }
    private void Start()
    {
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        videoPlayer.loopPointReached += OnVideoCompleted;
        rawImage.enabled = false; // Hide RawImage initially
    }
    private void OnDestroy()
    {
        videoPlayer.prepareCompleted -= OnPrepareCompleted;
        videoPlayer.loopPointReached -= OnVideoCompleted;
    }

    public void SwitchVideo(string filePath, bool isLoop, Action onPlaybackCompleted)
    {
        if (videoPlayer.isPlaying || videoPlayer.isPaused)
        {
            videoPlayer.Stop();
        }

        // 设置是否循环播放
        videoPlayer.isLooping = isLoop;

        if (!isLoop)
        {
            playbackCompletedAction = onPlaybackCompleted;
        }
        else
        {
            onPlaybackCompleted = null;
        }

        // 加载新的视频
        // ...

        videoPlayer.Prepare();
        rawImage.enabled = true; // Show RawImage when video is prepared
    }
    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }
    public void ResumeVideo()
    {
        if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
    }

    public void CloseVideo()
    {
        if (videoPlayer.isPlaying || videoPlayer.isPaused)
        {
            videoPlayer.Stop();
            rawImage.enabled = false; // Hide RawImage when video is closed
        }
    }

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        vp.Play();
    }
    private void OnVideoCompleted(VideoPlayer vp)
    {
        playbackCompletedAction?.Invoke();
        if (!videoPlayer.isLooping)
        {
            rawImage.enabled = false; // Hide RawImage when video is completed and not looping
        }
    }
}
