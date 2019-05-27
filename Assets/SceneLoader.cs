using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCg;
    [Range(0.5f, 2.0f)]
    public float fadeDuration = 1.0f;

    public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();
    void InitSceneInfo()
    {
        //loadScenes.Add("map1", LoadSceneMode.Single);
        loadScenes.Add("map2", LoadSceneMode.Single);
    }
    IEnumerator Start()
    {
        InitSceneInfo();

        fadeCg.alpha = 1.0f;
        Debug.Log('a');
        foreach (var _loadScene in loadScenes)
        {
            yield return StartCoroutine(LoadScene(_loadScene.Key, _loadScene.Value));
        }
        Debug.Log('a');
        StartCoroutine(Fade(0.0f));
    }

    IEnumerator LoadScene(string sceneName , LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);
        Debug.Log('a');
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
        Debug.Log('a');
    }
    IEnumerator Fade(float finalAlpha)
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("map2"));

        Debug.Log('a');
        fadeCg.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(fadeCg.alpha - finalAlpha) / fadeDuration;

        while(!Mathf.Approximately(fadeCg.alpha, finalAlpha))
        {
            Debug.Log('b');
            fadeCg.alpha = Mathf.MoveTowards(fadeCg.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log('c');
        fadeCg.blocksRaycasts = false;

        SceneManager.UnloadSceneAsync("SceneLoader");
        
    }

    
}
