using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Behaviour
{
    GameObject particleGroup ;      //배열로
    public ParticleSystem m_particleSystem;
    public void Init(string _link)
    {
        particleGroup = GameObject.Find(_link);
        m_particleSystem = particleGroup.GetComponent<ParticleSystem>();
    }

    public override void Work(TYPE _type)
    {
        
    }

    public void Activate(Vector3 _vec)
    {
        Debug.Log(m_particleSystem.time);
        m_particleSystem.playbackSpeed = 1.5f;
        m_particleSystem.transform.position = Check.Insert_Position_XZ(_vec, 0.2f);
        m_particleSystem.Play();
    }
   

}
