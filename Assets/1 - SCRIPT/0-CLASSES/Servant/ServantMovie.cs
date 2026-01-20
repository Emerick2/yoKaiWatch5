using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class ServantMovie {
    public bool finishedMovie;
    private VideoPlayer videoPlayer;
    private RawImage displayArea;
    private GameObject canvasObj;

    public void StartMovie(MonoBehaviour runner, string moviePath){
        VideoClip clip = Resources.Load<VideoClip>(moviePath);
        
        if (clip == null) {
            Debug.LogError("Vidéo non trouvée dans Resources : " + moviePath);
            return;
        }

        SetupVideoSystem(clip);

        finishedMovie = false;
        videoPlayer.Play();

        runner.StartCoroutine(WatchingMovie((float)clip.length));
    }

    private void SetupVideoSystem(VideoClip clip){
        canvasObj = new GameObject("VideoCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();

        GameObject uiImage = new GameObject("VideoDisplay");
        uiImage.transform.SetParent(canvasObj.transform, false);
        displayArea = uiImage.AddComponent<RawImage>();
        
        RectTransform rect = uiImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        videoPlayer = canvasObj.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = clip;

        RenderTexture videoTexture = new RenderTexture((int)clip.width, (int)clip.height, 16);
        videoPlayer.targetTexture = videoTexture;
        displayArea.texture = videoTexture;
    }

    private IEnumerator WatchingMovie(float duration){
        Debug.Log(duration);
        yield return new WaitForSeconds(duration);
        End();
    }

    public void End(){
        finishedMovie = true;
        if (canvasObj != null){
            Object.Destroy(canvasObj);
        }
        Debug.Log("Vidéo terminée !");
    }
}