using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    GameSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }

    void FixedUpdate()
    {
        Vector3 vec = system.pPlayer2.transform.position;   
        if (system.pPlayer2.GetIsAttackMode())
        {


            Vector3 vec2 = system.MousePoint();
            Vector3 sq = vec;
            if (Mathf.Abs(vec2.x - vec.x) > 2.5f)
            {
                sq.x = Mathf.Abs(vec2.x - (vec.x + 2.5f)) / 2;
                if (vec2.x >= vec.x)
                {
                    sq.x = sq.x + vec.x;

                }
                else
                {
                    sq.x = sq.x + vec2.x;
                }
                if (sq.x > system.pPlayer2.transform.position.x + 3.0f)
                {
                    sq.x = system.pPlayer2.transform.position.x + 3.0f;
                }
                else if (sq.x < system.pPlayer2.transform.position.x - 3.0f)
                {
                    sq.x = system.pPlayer2.transform.position.x - 3.0f;
                }
            }
            if (Mathf.Abs(vec2.z - vec.z) > 2.0f)
            {
                sq.z = Mathf.Abs(vec2.z - (vec.z + 2.0f)) / 2;

                if (vec2.z >= vec.z)
                {
                    sq.z = sq.z + vec.z;

                }
                else
                {
                    sq.z = sq.z + vec2.z;
                }
                if (sq.z > system.pPlayer2.transform.position.z + 1.5f)
                {
                    sq.z = system.pPlayer2.transform.position.z + 1.5f;
                }
                else if (sq.z < system.pPlayer2.transform.position.z - 1.5f)
                {
                    sq.z = system.pPlayer2.transform.position.z - 1.5f;
                }
            }
            float x = Mathf.Lerp(transform.position.x, sq.x, Time.deltaTime * 1.5f);
            float z = Mathf.Lerp(transform.position.z, sq.z, Time.deltaTime * 1.5f);
            sq.x = x;
            sq.z = z;
            transform.position = sq;
        }
        else
        {
            float x = Mathf.Lerp(transform.position.x, system.pPlayer2.transform.position.x, Time.deltaTime * 1.5f);
            float z = Mathf.Lerp(transform.position.z, system.pPlayer2.transform.position.z, Time.deltaTime * 1.5f);
            vec.x = x;
            vec.z = z;
            transform.position = vec;
        }
    }

}
