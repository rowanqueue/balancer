using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;

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
    public Line line;
    public Line lineHighlight;
    public Disc disc;
    public Disc discHighlight;
    public TextMeshPro numberDisplay;
    public List<Disc> discs;
    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        shape = GetComponentInChildren<RegularPolygon>();
        shapeHighlight = shape.transform.GetChild(0).GetComponent<RegularPolygon>();
        line = GetComponentInChildren<Line>();
        lineHighlight = line.transform.GetChild(0).GetComponent<Line>();
        disc = GetComponentInChildren<Disc>();
        discHighlight = disc.transform.GetChild(0).GetComponent<Disc>();
        numberDisplay = GetComponentInChildren<TextMeshPro>();
        discs = new List<Disc>();
        discs.Add(disc);
        foreach(Transform child in transform.GetChild(4)){
            discs.Add(child.GetComponent<Disc>());
        }
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
            if(boop.getBigger){
                _number+=1;
            }
        }
        numberDisplay.text = _number.ToString();
        numberDisplay.enabled = number > 0;
        numberDisplay.color = Services.GameController.black;
        for(int i = 1; i < discs.Count;i++){
            discs[i].Radius = disc.Radius;
            discs[i].Color = Services.GameController.colors[colorNumber];
            if(i > number){
                discs[i].enabled = false;
                continue;
            }
            discs[i].enabled = false;
            /*if(number == 1){
                discs[i].transform.localPosition = Vector3.zero;
                continue;
            }*/
            float angle = (360f/(number))*i;
            switch(number){
                case 1:
                    angle+=90f;
                    break;
                case 3:
                    angle-=30f;
                    break;
                case 4:
                    angle+=45f;
                    break;
                case 5:
                    angle+=18f;
                    break;
            }
            float distance = discs[i].Radius*1.25f;
            discs[i].transform.localPosition = new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad),0f)*distance;
            discs[i].SortingOrder = disc.SortingOrder-1;
        }
        float scale = 0.175f;//0.35f;
        scale+=((_number-1)*0.035f);
        scale = Mathf.Lerp(0.185f,0.425f,((float)_number/9f));
        if(Services.GameController.inTutorial && Services.GameController.tutorialStage == (int)TutorialStage.Grabing){
            scale+=0.025f*(Mathf.Sin(Time.time*5f)+1f);
        }
        disc.Radius = scale;
        discHighlight.Radius = disc.Radius+0.05f;
        
        _number = 1;
        
        /*if(_number == 1){
            numberDisplay.transform.localPosition = new Vector3(0.01f,-0.02f,0f);
        }else{
            numberDisplay.transform.localPosition = new Vector3(0.01f,-0.02f,0f);
        }*/
        shape.enabled = false;
        line.enabled = false;
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
        if(_number==3){
            shape.transform.localPosition = new Vector3(0,-0.025f,0f);
        }else{
            shape.transform.localPosition = Vector3.zero;
        }
        if(_number >= 3){
            shape.enabled = true;
            shape.Sides = _number;
        }else if(_number == 2){
            line.enabled = true;
        }else{
            disc.enabled = true;
        }
        
        
        Color color = Services.GameController.colors[colorNumber];
        shape.Color = color;
        line.Color = color;
        disc.Color = color;
        //disc.Color = Services.GameController.white;
        shape.SortingOrder = depth;
        line.SortingOrder = depth;
        disc.SortingOrder = depth;
        numberDisplay.sortingOrder = depth;


        shapeHighlight.enabled = _number >=3 && isSelected;
        shapeHighlight.SortingOrder = shape.SortingOrder-1;
        shapeHighlight.Sides = shape.Sides;
        shapeHighlight.Angle = shape.Angle;

        lineHighlight.enabled = _number == 2 && isSelected;
        lineHighlight.SortingOrder = line.SortingOrder-1;

        discHighlight.enabled = _number == 1 && isSelected;
        discHighlight.SortingOrder = disc.SortingOrder-1;
    }
}
