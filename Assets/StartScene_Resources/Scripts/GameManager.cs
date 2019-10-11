using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public TimelineManager timelineManager;
    public LoadManager loadManager;
    public Text loadGameText;
    public Button loadGameBtn;
    
    void Start()
    {                
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        StateLoadBtn();

        if (Input.anyKeyDown && !timelineManager.isSkip)
        {
           timelineManager.SkipTimeline();
        }                
    }

   public void NewGameBtn()
   {        
        SceneManager.LoadScene("map1");
        loadManager.SetStageLevel();
        loadManager.Save();
        
   }

   public void LoadGameBtn()
   {
        SceneManager.LoadScene("map" + loadManager.GetStageLevel());        
    }
   public void ExitGameBtn()
   {
        Application.Quit();
   }
    
   public void StateLoadBtn()
   {
        if(loadManager.GetStageLevel() > 0)
        {
            loadGameBtn.enabled = true;
            loadGameText.color = new Color(253 / 255f, 190 / 255f, 36 / 255f);
        }
        else
        {
            loadGameBtn.enabled = false;
            loadGameText.color = new Color(137 / 255f, 104 / 255f, 24 / 255f);
        }
   }
}
