using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using MoreMountains.NiceVibrations;

public class SettingsMenu : MonoBehaviour
{
    public bool active;
    public bool clicked;
    public BoxCollider2D collider;
    float scale;
    Vector3 cameraPos;
    float x;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        cameraPos = Camera.main.transform.position;
        x = transform.position.x;
        scale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = Services.GameController.black;   
        if(Services.GameController.inTutorial && Services.GameController.tutorialStage < (int)TutorialStage.Undo){
            transform.position = new Vector2(x+1,transform.position.y);
        }else{
            transform.position = new Vector2(x,transform.position.y);
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetMouseButtonDown(0)){
            if(collider.OverlapPoint(mousePos)){
                active = !active;
                clicked = true;
                Services.AudioManager.PlaySound(Services.AudioManager.select);
                MMVibrationManager.Haptic (HapticTypes.MediumImpact);
            }
            
        }
        Vector3 _pos = cameraPos;
        if(active){
            _pos+=Vector3.up*1.75f;
        }
        Camera.main.transform.position+=(_pos-Camera.main.transform.position)*(0.2f*Time.deltaTime/(1/60f));
        float _scale = scale;
        if(clicked){
            _scale*= 0.8f;
            if(transform.localScale.x < 1.02f*_scale){
                clicked = false;
            }
        }
        transform.localScale += ((Vector3.one*_scale)-transform.localScale)*(0.2f*Time.deltaTime/(1/60f));
    }
}
