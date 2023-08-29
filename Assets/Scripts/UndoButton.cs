using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class UndoButton : MonoBehaviour
{
    public bool active;
    public bool clicked;
    public Rectangle rect;
    public RegularPolygon arrowHead;
    public Rectangle arrowBody;
    public BoxCollider2D collider;
    float scale;
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        active = Services.GameController.turns.Count > 0 && Services.GameController.gameOver == false;
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
                if(collider.OverlapPoint(mousePos) && Services.GameController.bigPoints >= 1){
                    Services.GameController.Undo();
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
        arrowHead.Color = Color.Lerp(arrowHead.Color,c,0.3f*(Time.deltaTime/(1/60f)));
        arrowBody.Color =  arrowHead.Color;
        rect.Color = Services.GameController.black;
    }
}
