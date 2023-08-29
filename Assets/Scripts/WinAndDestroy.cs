using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAndDestroy : MonoBehaviour
{
    float speed = 0.15f;
    float scale = 1f;
    Vector3 large = Vector3.one*1.25f;
    bool getBigger = true;
    float bigTime = 0f;
    public int depth = 0;
    // Start is called before the first frame update
    void Awake()
    {
        scale = transform.localScale.x;
        ExpandAndRemove ear = GetComponent<ExpandAndRemove>();
        if(ear != null){
            Destroy(ear);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(getBigger){
            transform.localScale+=((large*scale)-transform.localScale)*0.2f*Time.deltaTime/(1f/60f);
            if(transform.localScale.x > 0.98f*large.x*scale){
                transform.localScale = large*scale;
                getBigger = false;
                bigTime = Time.time;
                //Services.AudioManager.PlaySound(Services.AudioManager.pop,depth);
            }
        }else{
            if(Time.time > bigTime+0.12f){
                transform.localScale+=((Vector3.zero)-transform.localScale)*0.15f*Time.deltaTime/(1f/60f);
                if(transform.localScale.x < 0.1f){
                    //transform.localScale = Vector3.zero*scale;
                    GameObject.Destroy(gameObject);
                }
            }
            
        }
    }
}
