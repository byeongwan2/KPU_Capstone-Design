using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PRDemoLoad : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Player1Start") || Input.GetButtonDown("Player2Start") || Input.GetButtonDown("Player3Start") || Input.GetButtonDown("Player4Start"))
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

}
