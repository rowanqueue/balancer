using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopUp : MonoBehaviour
{
    public int number = 1;
    public int multiplier = 1;
    float scale = 0.1f;
    bool getBigger = true;
    float bigTime;
    TextMeshPro numberDisplay;
    // Start is called before the first frame update
    void Awake()
    {
        scale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        numberDisplay = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        numberDisplay.color = Services.GameController.white;
        numberDisplay.text = "+"+number.ToString();
        
        /*if(number < 0){
            numberDisplay.text = "GRID CLEAR!\n+10";
        }
        if(multiplier > 1){
            numberDisplay.text+="<size=25>x</size>"+multiplier.ToString();
        }*/
        if(number < 0){
            numberDisplay.text = "GRID CLEAR!\n+";
        }else{
            numberDisplay.text ="+";
        }
        numberDisplay.text+=((number < 0 ? 9 : number)*multiplier).ToString();

        if(getBigger){
            transform.localScale+=((Vector3.one*scale)-transform.localScale)*0.1f*Time.deltaTime/(1f/60f);
            transform.localPosition+=Vector3.up*0.01f*Time.deltaTime/(1f/60f);
            if(transform.localScale.x > 0.98f*scale){
                transform.localScale = Vector3.one*scale;
                getBigger = false;
                bigTime = Time.time;
            }
        }else{
            if(Time.time > bigTime+0.01f){
                transform.localScale+=((Vector3.zero*scale)-transform.localScale)*0.2f*Time.deltaTime/(1f/60f);
                transform.localPosition-=Vector3.up*0.005f*Time.deltaTime/(1f/60f);
                if(transform.localScale.x < 0.02f*scale){
                    transform.localScale = Vector3.zero*scale;
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
