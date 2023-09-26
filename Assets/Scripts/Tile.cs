using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class Tile : MonoBehaviour
{
    public bool rightWall;
    public bool upWall;
    public bool downWall => position.y > 0 && Services.GameController.tiles[position+Vector2Int.down].upWall;
    public bool leftWall => position.x > 0 && Services.GameController.tiles[position+Vector2Int.left].rightWall;
    public Rectangle top;
    public Disc topRight;
    public Rectangle right;
    public Disc botRight;
    public Rectangle bottom;
    public Disc botLeft;
    public Rectangle left;
    public Disc topLeft;
    public Vector2Int position;
    /*bool hasRight = true;
    public Rectangle rightLine;
    bool hasUp = true;
    public Rectangle upLine;*/
    // Start is called before the first frame update
    void Start()
    {
        /*if(position.x >= Services.GameController.gridSize.x-1){
            rightLine.enabled = false;
            hasRight = false;
        }
        if(position.y >= Services.GameController.gridSize.y-1){
            upLine.enabled = false;
            hasUp = false;
        }
        rightLine.Color = Services.GameController.black;
        upLine.Color = Services.GameController.black;*/
    }

    // Update is called once per frame
    void Update()
    {
        int colorNumber = -1;
        if(Services.GameController.grid.ContainsKey(position)){
            colorNumber = Services.GameController.grid[position].colorNumber;
            if(Services.GameController.grid[position].locked){
                colorNumber = -1;
            }
        }
        int[] numbers = new int[8]{-1,-1,-1,-1,-1,-1,-1,-1};
        for(int i = 0; i < Services.GameController.allDirections.Length;i++){
            if(Services.GameController.grid.ContainsKey(position+Services.GameController.allDirections[i])){
                numbers[i] = Services.GameController.grid[position+Services.GameController.allDirections[i]].colorNumber;
                if(Services.GameController.grid[position+Services.GameController.allDirections[i]].locked){
                    numbers[i] = -1;
                }
                numbers[i] = -1;
            }
        }
        Color topColor = Services.GameController.black;
        Color trColor = Services.GameController.black;
        Color rightColor = Services.GameController.black;
        Color brColor = Services.GameController.black;
        Color bottomColor = Services.GameController.black;
        Color blColor = Services.GameController.black;
        Color leftColor = Services.GameController.black;
        Color tlColor = Services.GameController.black;
        //TR
        if(position.x == Services.GameController.gridSize.x-1 && position.y == Services.GameController.gridSize.y-1){
            trColor = Services.GameController.black;
        }else{
            trColor = Services.GameController.clear;
            if(numbers[0] != -1){
                if(numbers[0] == numbers[1] && numbers[0] == numbers[2]){
                    trColor = Services.GameController.black;
                }
            }
        }
        if(colorNumber != -1 && numbers[0] == colorNumber && numbers[1] == colorNumber && numbers[2] == colorNumber){
            trColor = Services.GameController.clear;
        }
        //BR
        if(position.x == Services.GameController.gridSize.x-1 && position.y == 0){
            brColor = Services.GameController.black;
        }else{
            brColor = Services.GameController.clear;
            if(numbers[2] != -1){
                if(numbers[2] == numbers[3] && numbers[2] == numbers[4]){
                    brColor = Services.GameController.black;
                }
            }
        }
        if(colorNumber != -1 && numbers[2] == colorNumber && numbers[3] == colorNumber && numbers[4] == colorNumber){
            brColor = Services.GameController.clear;
        }
        //BL
        if(position.x == 0 && position.y == 0){
            blColor = Services.GameController.black;
        }else{
            blColor = Services.GameController.clear;
            if(numbers[4] != -1){
                if(numbers[4] == numbers[5] && numbers[4] == numbers[6]){
                    blColor = Services.GameController.black;
                }
            }
        }
        if(colorNumber != -1 && numbers[4] == colorNumber && numbers[5] == colorNumber && numbers[6] == colorNumber){
            blColor = Services.GameController.clear;
        }
        //TL
        if(position.x == 0 && position.y == Services.GameController.gridSize.y-1){
            tlColor = Services.GameController.black;
        }else{
            tlColor = Services.GameController.clear;
            if(numbers[6] != -1){
                if(numbers[6] == numbers[7] && numbers[6] == numbers[0]){
                    tlColor = Services.GameController.black;
                }
            }
        }
        if(colorNumber != -1 && numbers[6] == colorNumber && numbers[7] == colorNumber && numbers[0] == colorNumber){
            tlColor = Services.GameController.clear;
        }
        //TOP
        if(tlColor == Services.GameController.clear && (numbers[6] != -1 && numbers[6] == numbers[7] && numbers[6] == colorNumber) == false){
            top.transform.localPosition = new Vector2(-0.5f,top.transform.localPosition.y);
            top.Width = 0.75f;
        }else{
            top.transform.localPosition = new Vector2(-0.25f,top.transform.localPosition.y);
            top.Width = 0.5f;
        }
        if(trColor == Services.GameController.clear){
            top.Width += 0.25f;
            if(numbers[2] != -1 && numbers[2] == numbers[1] && numbers[2] == colorNumber){
                top.Width -= 0.25f;
            }
        }
        if(numbers[0] != -1 && numbers[0] == colorNumber){
            topColor = Services.GameController.clear;
        }
        if(upWall){
            topColor = Services.GameController.white;
        }
        //RIGHT
        if(brColor == Services.GameController.clear && (numbers[4] != -1 && numbers[4] == numbers[3] && numbers[4] == colorNumber) == false){
            right.transform.localPosition = new Vector2(right.transform.localPosition.x,-0.5f);
            right.Height = 0.75f;
        }else{
            right.transform.localPosition = new Vector2(right.transform.localPosition.x,-0.25f);
            right.Height = 0.5f;
        }
        if(trColor == Services.GameController.clear){
            right.Height += 0.25f;
            if(numbers[0] != -1 && numbers[0] == numbers[1] && numbers[0] == colorNumber){
                right.Height -= 0.25f;
            }
        }
        if(numbers[2] != -1 && numbers[2] == colorNumber){
            rightColor = Services.GameController.clear;
        }
        if(rightWall){
            rightColor = Services.GameController.black;
        }
        //BOTTOM
        if(blColor == Services.GameController.clear && (numbers[6] != -1 && numbers[6] == numbers[5] && numbers[6] == colorNumber) == false){
            bottom.transform.localPosition = new Vector2(-0.5f,bottom.transform.localPosition.y);
            bottom.Width = 0.75f;
        }else{
            bottom.transform.localPosition = new Vector2(-0.25f,bottom.transform.localPosition.y);
            bottom.Width = 0.5f;
        }
        if(brColor == Services.GameController.clear){
            bottom.Width += 0.25f;
            if(numbers[2] != -1 && numbers[2] == numbers[3] && numbers[2] == colorNumber){
                bottom.Width -= 0.25f;
            }
        }
        if(numbers[4] != -1 && numbers[4] == colorNumber){
            bottomColor = Services.GameController.clear;
        }
        if(downWall){
            bottomColor = Services.GameController.white;
        }
        //LEFT
        if(blColor == Services.GameController.clear && (numbers[4] != -1 && numbers[4] == numbers[5] && numbers[4] == colorNumber) == false){
            left.transform.localPosition = new Vector2(left.transform.localPosition.x,-0.5f);
            left.Height = 0.75f;
        }else{
            left.transform.localPosition = new Vector2(left.transform.localPosition.x,-0.25f);
            left.Height = 0.5f;
        }
        if(tlColor == Services.GameController.clear){
            left.Height += 0.25f;
            if(numbers[0] != -1 && numbers[0] == numbers[7] && numbers[0] == colorNumber){
                left.Height -= 0.25f;
            }
        }
        if(numbers[6] != -1 && numbers[6] == colorNumber){
            leftColor = Services.GameController.clear;
        }
        if(leftWall){
            leftColor = Services.GameController.black;
        }
        /*int upColor = -1;
        if(hasUp && Services.GameController.grid.ContainsKey(position)){
            if(Services.GameController.grid.ContainsKey(position+Vector2Int.up)){
                if(Services.GameController.grid[position].colorNumber == Services.GameController.grid[position+Vector2Int.up].colorNumber){
                    upColor = Services.GameController.grid[position].colorNumber;
                    upColor = 4;
                }
            }
        }*/

        /*int rightColor = -1;
        if(hasRight && Services.GameController.grid.ContainsKey(position)){
            if(Services.GameController.grid.ContainsKey(position+Vector2Int.right)){
                if(Services.GameController.grid[position].colorNumber == Services.GameController.grid[position+Vector2Int.right].colorNumber){
                    rightColor = Services.GameController.grid[position].colorNumber;
                    rightColor = 4;
                }
            }
        }
        int upColor = -1;
        if(hasUp && Services.GameController.grid.ContainsKey(position)){
            if(Services.GameController.grid.ContainsKey(position+Vector2Int.up)){
                if(Services.GameController.grid[position].colorNumber == Services.GameController.grid[position+Vector2Int.up].colorNumber){
                    upColor = Services.GameController.grid[position].colorNumber;
                    upColor = 4;
                }
            }
        }
        float lerpSpeed = 0.25f;
        if(rightColor >= 0){
            rightLine.Color = Color.Lerp(rightLine.Color,Services.GameController.colors[rightColor],lerpSpeed*(Time.deltaTime/(1f/60f)));
        }else{
            rightLine.Color = Color.Lerp(rightLine.Color,Services.GameController.clear,lerpSpeed*(Time.deltaTime/(1f/60f)));
        }
        if(upColor >= 0){
            upLine.Color = Color.Lerp(upLine.Color,Services.GameController.colors[upColor],lerpSpeed*(Time.deltaTime/(1f/60f)));
        }else{
            upLine.Color = Color.Lerp(upLine.Color,Services.GameController.clear,lerpSpeed*(Time.deltaTime/(1f/60f)));
        }
        bool fullSquare = false;
        if(rightColor >= 0 && upColor >= 0){
            if(Services.GameController.grid.ContainsKey(position+Vector2Int.right+Vector2Int.up)){
                if(Services.GameController.grid[position].colorNumber == Services.GameController.grid[position+Vector2Int.right+Vector2Int.up].colorNumber){
                    
                    fullSquare  =true;
                }
            }
        }
        if(fullSquare){
            upLine.Width += (1f-upLine.Width)*0.2f*(Time.deltaTime/(1f/60f));
        }else{
            upLine.Width += (0.8f-upLine.Width)*0.2f*(Time.deltaTime/(1f/60f));
        }*/
        float lerpSpeed = 2f*(Time.deltaTime/(1f/60f));
        top.Color = Color.Lerp(top.Color,topColor,lerpSpeed);
        topRight.Color = Color.Lerp(topRight.Color,trColor,lerpSpeed);
        right.Color = Color.Lerp(right.Color,rightColor,lerpSpeed);
        botRight.Color = Color.Lerp(botRight.Color,brColor,lerpSpeed);
        bottom.Color = Color.Lerp(bottom.Color,bottomColor,lerpSpeed);
        botLeft.Color = Color.Lerp(botLeft.Color,blColor,lerpSpeed);
        left.Color = Color.Lerp(left.Color,leftColor,lerpSpeed);
        topLeft.Color = Color.Lerp(topLeft.Color,tlColor,lerpSpeed);
    }
}
