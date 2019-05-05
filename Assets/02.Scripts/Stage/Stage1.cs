using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    private Alien create_start_monster;
    public Transform create_monster_position;
    // Start is called before the first frame update
    private GameObject robot;
    void Start()
    {
        create_start_monster = GameObject.Find("Alien").GetComponent<Alien>();
        robot = GameObject.Find("Robot");
    }

    public void CreateRobotFirst()
    {
        StartCoroutine(CreateRobot());
        
    }

    IEnumerator CreateRobot()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(2.0f);
           
            GameObject r =  Instantiate(robot);
            r.SetActive(true);
            r.transform.position = create_monster_position.position;
            Debug.Log("생성완료");
        }
        
    }
}
