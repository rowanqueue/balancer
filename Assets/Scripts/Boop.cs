using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boop : MonoBehaviour
{
    Vector3 large = Vector3.one*0.5f;
    public bool getBigger = true;
    float scale;
    float bigTime;
    public int depth = 0;
    public bool unlocking = false;
    // Start is called before the first frame update
    void Awake()
    {
        scale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(getBigger){
            transform.localScale+=((large*scale)-transform.localScale)*0.25f*Time.deltaTime/(1f/60f);
            if(transform.localScale.x < 1.02f*large.x*scale){
                transform.localScale = large*scale;
                getBigger = false;
                bigTime = Time.time;
                Services.AudioManager.PlaySound(Services.AudioManager.crack,depth);
            }
        }else{
            if(Time.time > bigTime+0.00f){
                transform.localScale+=((Vector3.one*scale)-transform.localScale)*0.3f*Time.deltaTime/(1f/60f);
                if(transform.localScale.x > 0.98f*scale){
                    transform.localScale = Vector3.one*scale;
                    Destroy(this);
                }
            }
            
        }
        /*if(getBigger){
            transform.localScale+=((large*scale)-transform.localScale)*0.25f*Time.deltaTime/(1f/60f);
            if(transform.localScale.x > 0.98f*large.x*scale){
                transform.localScale = large*scale;
                getBigger = false;
                bigTime = Time.time;
                Services.AudioManager.PlaySound(Services.AudioManager.crack);
            }
        }else{
            if(Time.time > bigTime+0.00f){
                transform.localScale+=((Vector3.one*scale)-transform.localScale)*0.3f*Time.deltaTime/(1f/60f);
                if(transform.localScale.x < 1.02f*scale){
                    transform.localScale = Vector3.one*scale;
                    Destroy(this);
                }
            }
            
        }*/
    }
}
