using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class RestartButton : MonoBehaviour
{
    public bool active;
    public bool clicked;
    public BoxCollider2D collider;
    float scale;
    float x;
    public Rectangle rect;
    public SpriteRenderer restart;
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.x;
        x  = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Services.GameController.inTutorial && Services.GameController.tutorialStage < (int)TutorialStage.Undo){
            transform.localPosition = new Vector2(x-1,transform.localPosition.y);
        }else{
            transform.localPosition = new Vector2(x,transform.localPosition.y);
        }
        if(Input.GetMouseButtonDown(0)){
            if(collider.OverlapPoint(mousePos) && Services.GameController.bigPoints >= 5){
                Services.GameController.Restart();
                clicked = true;
                Services.AudioManager.PlaySound(Services.AudioManager.select);
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
        restart.color = Services.GameController.white;
    }
}
