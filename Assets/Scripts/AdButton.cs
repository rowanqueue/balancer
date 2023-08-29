using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;
using System;

public class AdButton : MonoBehaviour
{
    public bool active;
    public bool clicked;
    public Rectangle rect;
    public BoxCollider2D collider;
    public GameObject ad;
    float scale;
    public TextMeshPro adTime;
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        ad.SetActive(Services.GameController.watchingAd);
        adTime.text = "WATCHING\nAD\n"+Mathf.FloorToInt((Services.GameController.adTimeOver-Time.time)+1f).ToString();
        //adTime.color = Services.GameController.black;
        
        active = Services.GameController.watchingAd == false;
       /* if(Services.GameController.inTutorial && Services.GameController.tutorialStage < (int)TutorialStage.Undo){
            transform.localPosition = new Vector2(-2f,transform.localPosition.y);
        }else{
            transform.localPosition = new Vector2(-1f,transform.localPosition.y);
        }*/
        Color c = Services.GameController.white;
        if(active == false){
            c = Services.GameController.clear;
        }else{
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0)){
                if(collider.OverlapPoint(mousePos)){
                    Services.GameController.WatchAd();
                    clicked = true;
                    Services.AudioManager.PlaySound(Services.AudioManager.select);
                }
                
            }
            
        }
        float _scale = scale;
        if(clicked){
            _scale*= 0.8f;
            if(transform.localScale.x < 0.82f){
                clicked = false;
            }
        }
        transform.localScale += ((Vector3.one*_scale)-transform.localScale)*(0.2f*Time.deltaTime/(1/60f));
        rect.Color = Services.GameController.black;
    }
}
