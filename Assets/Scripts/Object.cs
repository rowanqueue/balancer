using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;
using System;

public class Object : MonoBehaviour
{
    public Vector2Int position;
    public int depth = 0;
    public int number = 3;
    public int colorNumber = 0;
    public bool isOption = false;
    public bool isSelected = false;
    public BoxCollider2D collider;
    public RegularPolygon shape;
    public RegularPolygon shapeHighlight;
    public List<Line> lines;
    public List<RegularPolygon> lineEnds;
    public Disc disc;
    public Disc discHighlight;
    public TextMeshPro numberDisplay;
    public bool isPopping = false;
    
    public int lockDirection = -1;
    public bool locked => lockDirection >= 0;
    public int howUnlocked = 2;
    public bool diagonal = false;
    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        shape = GetComponentInChildren<RegularPolygon>();
        shapeHighlight = shape.transform.GetChild(0).GetComponent<RegularPolygon>();
        disc = GetComponentInChildren<Disc>();
        discHighlight = disc.transform.GetChild(0).GetComponent<Disc>();
        numberDisplay = GetComponentInChildren<TextMeshPro>();
    }
    public bool CanConnectToLocked(int fromDirection){
        if(locked == false){
            return true;
        }
        Debug.Log("yeehaw"+fromDirection.ToString());
        int opp = Services.GameController.OppositeDirection(fromDirection);
        Debug.Log(opp);
        for(int i = 0; i < howUnlocked;i++){
            Debug.Log((lockDirection+i)%4);
            if(opp == (lockDirection+i)%4){
                return true;
            }
        }
        return false;
    }
    public bool CanConnectOut(int direction){
        if(locked == false){
            return true;
        }
        for(int i = 0; i <howUnlocked;i++){
            if(direction == (lockDirection+i)%4){
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(number == 0){
            return;
        }*/
        //numberDisplay.enabled = false;
        int _number = number;
        Boop boop = gameObject.GetComponent<Boop>();
        if(boop != null){
            if(boop.unlocking == false && boop.getBigger){
                _number+=1;
            }
        }
        numberDisplay.text = _number.ToString();
        numberDisplay.enabled = number > 0;
        numberDisplay.color = Services.GameController.black;

        float scale = 0.175f;//0.35f;
        scale+=((_number-1)*0.035f);
        scale = Mathf.Lerp(0.185f,0.425f,((float)_number/9f));
        if(Services.GameController.inTutorial && Services.GameController.tutorialStage == (int)TutorialStage.Grabing){
            scale+=0.025f*(Mathf.Sin(Time.time*5f)+1f);
        }
        disc.Radius = scale;
        discHighlight.Thickness = disc.Radius*0.35f;
        discHighlight.Radius = disc.Radius-discHighlight.Thickness*0.5f;
        
        _number = 1;
        if(diagonal){
            _number = 4;
        }
        
        /*if(_number == 1){
            numberDisplay.transform.localPosition = new Vector3(0.01f,-0.02f,0f);
        }else{
            numberDisplay.transform.localPosition = new Vector3(0.01f,-0.02f,0f);
        }*/
        shape.enabled = false;
        //line.enabled = false;
        disc.enabled = false;
        if(_number%2 == 0){
            shape.Angle = (360/_number)*Mathf.Deg2Rad;
            if(_number == 4){
                shape.Angle = 45*Mathf.Deg2Rad;
            }
            if(_number == 8){
                shape.Angle = 22.5f*Mathf.Deg2Rad;
            }
        }else{
            shape.Angle = 90*Mathf.Deg2Rad;
        }
        shape.Angle = 0f;
        if(_number==3){
            shape.transform.localPosition = new Vector3(0,-0.025f,0f);
        }else{
            shape.transform.localPosition = Vector3.zero;
        }
        if(_number >= 3){
            shape.enabled = true;
            shape.Sides = _number;
        }else if(_number == 2){
            //line.enabled = true;
        }else{
            disc.enabled = true;
        }
        
        
        Color color = Services.GameController.colors[colorNumber];
        
        shape.Color = color;
        //line.Color = color;
        disc.Color = color;
        //disc.Color = Services.GameController.white;
        shape.SortingOrder = depth;
        //line.SortingOrder = depth;
        disc.SortingOrder = depth;
        numberDisplay.sortingOrder = depth;


        shapeHighlight.enabled = _number >=3 && isSelected;
        shapeHighlight.SortingOrder = shape.SortingOrder-1;
        shapeHighlight.Sides = shape.Sides;
        shapeHighlight.Angle = shape.Angle;

        //lineHighlight.enabled = _number == 2 && isSelected;
        //lineHighlight.SortingOrder = line.SortingOrder-1;

        discHighlight.enabled = _number == 1 && isSelected;
        discHighlight.SortingOrder = disc.SortingOrder+1;
        if(locked){
            discHighlight.Type = DiscType.Arc;
            var angles = new int[4]{0,-90,-180,-270};
            discHighlight.transform.localEulerAngles = new Vector3(0,0,angles[lockDirection]);
            if(howUnlocked == 3){
                discHighlight.AngRadiansEnd = 1.25f*Mathf.PI;
            }else if(howUnlocked == 2){
                discHighlight.AngRadiansEnd = 1.75f*Mathf.PI;
            }
            else{
                discHighlight.AngRadiansEnd = 2.25f*Mathf.PI;
            }
            discHighlight.Color = Services.GameController.black;
            if(_number > 3){
                shapeHighlight.enabled = true;
                shapeHighlight.Color = Services.GameController.black;
            }else{
                discHighlight.enabled = true;
            }
           
        }else{
            discHighlight.Type = DiscType.Ring;
            discHighlight.Color = Services.GameController.white;
            shapeHighlight.Color = Services.GameController.white;
        }

        Vector2Int[] directions = (diagonal ? Services.GameController.diagonalDirections : Services.GameController.directions);
        for(int i = 0; i < 4; i++){
            lines[i].SortingOrder = disc.SortingOrder-1;
            lines[i].Color = color;
            lines[i].Start = Vector2.zero;
            lines[i].Thickness = 0.1f;
            Vector2 target = Vector2.zero;
            bool holding = isSelected == false && Services.GameController.objectHeld && Services.GameController.grid.ContainsKey(Services.GameController.mouseGridPos) == false &&Services.GameController.tiles.ContainsKey(Services.GameController.mouseGridPos) && Services.GameController.heldObject.CanConnectToLocked(i) && Services.GameController.heldObject.colorNumber == colorNumber && Services.GameController.mouseGridPos == position+directions[i];
            if((isOption == false || (isSelected && Services.GameController.tiles.ContainsKey(Services.GameController.mouseGridPos) && Services.GameController.grid.ContainsKey(Services.GameController.mouseGridPos) == false)) && isPopping == false && CanConnectOut(i)){
                Vector2Int _pos = position;
                if(isSelected){
                    _pos = Services.GameController.mouseGridPos;
                }
                if(Services.GameController.grid.ContainsKey(_pos+directions[i])){
                    if((Services.GameController.grid[_pos+directions[i]].CanConnectToLocked(i)) &&Services.GameController.grid[_pos+directions[i]].colorNumber == colorNumber){
                        target = (Vector2)directions[i]*0.5f;
                    }
                }
                if(holding){
                    target = (Vector2)directions[i]*0.5f;
                }
            }
            lines[i].End += (Vector3)(target-(Vector2)lines[i].End).normalized*0.05f*(Time.deltaTime/(1f/60f));
            if(Vector3.Distance(lines[i].End,(Vector3)target) < 0.05f){
                lines[i].End = target;
            }
            lineEnds[i].SortingOrder = lines[i].SortingOrder;
            lineEnds[i].Color = color;
            lineEnds[i].transform.localPosition = lines[i].End;
            lineEnds[i].transform.localEulerAngles = new Vector3(0f,0f,Vector2.SignedAngle(Vector2.right,(Vector2)lines[i].End));
        }
    }
}
