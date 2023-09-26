using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Shapes;
using TMPro;
public enum TutorialStage{
    Grabing,
    Placing,
    Popping,
    Options,
    Practice,
    Cracking,
    Undo,
    Combo,
    GameOver
}
public class GameController : MonoBehaviour
{
    public string devSave = "";
    public bool reset = false;
    public int version = 0;
    public int numOptions = 3;
    public bool inTutorial => tutorialStage < 0 || tutorialStage < 9;
    public int tutorialStage;
    public int howManyToChoose = 2;
    public Vector2Int generatorRange = new Vector2Int(1,6);
    public int numColors = 3;
    public int playedSoFar = 0;
    public Vector2Int gridSize;
    public GameObject gridPrefab;
    public GameObject objPrefab;
    public GameObject crackShotPrefab;
    List<Object> options;
    public bool objectHeld = false;
    public Object heldObject = null;
    public Vector2Int mouseGridPos = Vector2Int.zero;
    public Dictionary<Vector2Int,Object> grid = new Dictionary<Vector2Int, Object>();
    public Dictionary<Vector2Int,Tile> tiles = new Dictionary<Vector2Int, Tile>();
    public int points;
    public List<int> earnedPoints;
    [HideInInspector]
    public Vector2Int[] directions = new Vector2Int[4]{
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };
    [HideInInspector]
    public Vector2Int[] diagonalDirections = new Vector2Int[4]{
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1)
    };
    [HideInInspector]
    public Vector2Int[] allDirections = new Vector2Int[8]{
        Vector2Int.up,
        new Vector2Int(1,1),
        Vector2Int.right,
        new Vector2Int(1,-1),
        Vector2Int.down,
        new Vector2Int(-1,-1),
        Vector2Int.left,
        new Vector2Int(-1,1)
    };
    List<Object> same_number = new List<Object>();
    List<Vector2Int> same_color = new List<Vector2Int>();
    public TextMeshPro score;
    public List<Color> colors;
    public List<Color> shadowColors;
    public Line numOptionsLine;
    public Rectangle[] numOptionsDisplay;
    public float numOptionsDisplayWidth;
    public bool waitForPopping;
    public bool beforeCheck;
    public float lastPop;
    public GameObject scorePopUpPrefab;
    public bool gameOver;
    public Color black;
    public Color white;
    public Color background;
    public Color clear;
    public struct Piece{
        public int number;
        public int color;
        public Piece(int n, int c){
            number = n;
            color = c;
        }
    }
    public int howManyCopiesInBag = 2;
    public List<Piece> tetrisBag = new List<Piece>();
    float timePlayed;
    public TextMeshPro timer;
    public List<string> turns = new List<string>();
    public int highScore;
    public TextMeshPro highScoreDisplay;
    bool turnStarted = false;
    public List<string> tutorialTexts = new List<string>();
    public int level = 0;
    bool unlockAppear;
    public List<int> levelUnlocks;
    public List<string> levelUnlockDesc;
    public TextMeshPro pointsDisplay;
    public TextMeshPro undoCostDisplay;
    public TextMeshPro restartCostDisplay;
    public TextMeshPro addBonusDisplay;
    public int bigPoints;
    [HideInInspector]
    public bool watchingAd;
    [HideInInspector]
    public float adTimeOver;
    public int adBonus = 7;
    // Start is called before the first frame update
    
    void Start()
    {
        if(reset){
            PlayerPrefs.DeleteAll();
        }
        //Random.InitState(5);
        numOptionsDisplayWidth = numOptionsDisplay[0].Width;
        if(PlayerPrefs.HasKey("tutorial")){
            tutorialStage = PlayerPrefs.GetInt("tutorial");
        }
        if(PlayerPrefs.HasKey("version")){
            int this_version = PlayerPrefs.GetInt("version");
            if(this_version != version){
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("version",version);
                PlayerPrefs.Save();
            }
        }else{
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("version",version);
            PlayerPrefs.Save();
        }
        if(PlayerPrefs.HasKey("highScore")){
            highScore = PlayerPrefs.GetInt("highScore");
        }
        if(PlayerPrefs.HasKey("Points")){
            bigPoints = PlayerPrefs.GetInt("Points");
        }
        
        Application.targetFrameRate = 60;
        Services.GameController = this;
        Services.AudioManager = GameObject.FindObjectOfType<AudioManager>();
        Services.AudioManager.Initialize();
        Services.Visuals = GameObject.FindObjectOfType<Visuals>();
        Services.Visuals.Initialize();
        for(var x =0; x < gridSize.x;x++){
            for(var y =0; y< gridSize.y;y++){
                GameObject gridObj = GameObject.Instantiate(gridPrefab,transform.position,Quaternion.identity,transform);
                gridObj.transform.position = new Vector3(x,y,0);
                Tile tile = gridObj.GetComponent<Tile>();
                tile.position = new Vector2Int(x,y);
                tiles.Add(tile.position,tile);
                Rectangle r = gridObj.GetComponentInChildren<Rectangle>();
                float curve = 0.25f;
                if(x == 0 && y == 0){
                    r.CornerRadiii = new Vector4(curve,0,0,0);
                }
                if(x == 0 && y == gridSize.y-1){
                    r.CornerRadiii = new Vector4(0,curve,0,0);
                }
                if(x == gridSize.x-1 && y == gridSize.y-1){
                    r.CornerRadiii = new Vector4(0,0,curve,0);
                }
                if(x == gridSize.x-1 && y == 0){
                    r.CornerRadiii = new Vector4(0,0,0,curve);
                }
            }
        }
        //Camera.main.transform.position = new Vector3(-0.5f+(float)gridSize.x/2f,1.25f/*-0.5f+(float)gridSize.y/2f*/,-10f);
        MakeOptions();
        if(PlayerPrefs.HasKey("lastBoard")){
            string save = PlayerPrefs.GetString("lastBoard");
            LoadBoardFromString(save);
        }
        if(inTutorial && tutorialStage == (int)TutorialStage.Grabing){
            options[1].number = 4;
            options[1].colorNumber = 0;
        }
        if(devSave != ""){
            LoadBoardFromString(devSave);
        }
        
        
    }
    void RemoveOptions(){
        
        foreach(Object o in options){
            if(o == null){
                continue;
            }
            if(o.isOption){
                o.depth = -20;
                o.gameObject.AddComponent<ShrinkAndDestroy>();
                //GameObject.Destroy(o.gameObject);
            }
        }
        options.Clear();
    }
    void ShuffleBag(){
        int _numColors = 2;
        if(level > 0){
            _numColors = 3;
        }
        for(var i = 0; i < _numColors; i++){
            int additional = 0;
            if(level > 1){
                additional = 1;
            }
            if(level > 2){
                additional = 2;
            }
            if(level > 3){
                additional = 3;
            }
            if(level > 4){
                additional = 4;
            }
            for(var j = generatorRange.x;j<generatorRange.y+1+additional;j++){
                int _numCopies = howManyCopiesInBag;
                for(var k = 0; k < _numCopies; k++){
                    tetrisBag.Add(new Piece(j,i));
                }
            }
        }
        tetrisBag = tetrisBag.OrderBy(x=> Random.value).ToList();
    }
    public void WatchAd(){
        watchingAd = true;
        adTimeOver = Time.time+5;
    }
    void MakeOptions(){
        options = new List<Object>();
        List<Piece> pieces = new List<Piece>();
        for(var i = 0; i < numOptions;i++){
            if(tetrisBag.Count == 0){
                ShuffleBag();
            }
            GameObject obj = GameObject.Instantiate(objPrefab,transform.position,Quaternion.identity,transform);
            obj.transform.position = OptionPlace(i);
            obj.AddComponent<ExpandAndRemove>();
            options.Add(obj.GetComponent<Object>());
            options[i].isOption = true;
            options[i].depth = 10;
            Piece nextPiece = tetrisBag[0];
            int index = 0;
            do{
                bool doubled = false;
                foreach(Piece _piece in pieces){
                    if(nextPiece.number ==_piece.number && nextPiece.color == _piece.color){
                        doubled = true;
                        break;
                    }
                }
                if(doubled == false){
                    break;
                }
                index+=1;
                if(index >= tetrisBag.Count-1){
                    ShuffleBag();
                    index = 0;
                }
                nextPiece = tetrisBag[index];
            }while(index < tetrisBag.Count-1);
            /*int index = 1;
            Piece nextPiece = tetrisBag[tetrisBag.Count-index];
            do{
                bool doubled = false;
                foreach(Piece _piece in pieces){
                    if(nextPiece.number ==_piece.number && nextPiece.color == _piece.color){
                        doubled = true;
                        break;
                    }
                }
                if(doubled == false){
                    break;
                }
                index+=1;
                if(index >= tetrisBag.Count){
                    ShuffleBag();
                    index = 1;
                }
                nextPiece = tetrisBag[tetrisBag.Count-index];
            }while(index < tetrisBag.Count);*/
            pieces.Add(nextPiece);
            tetrisBag.Remove(nextPiece);
            options[i].number = nextPiece.number;//Random.Range(generatorRange.x,generatorRange.y+1);
            if(options[i].number == 7){
                options[i].number = 0;
            }
            if(options[i].number == 8){
                options[i].number = Random.Range(1,6);
                options[i].lockDirection = Random.Range(0,3);
                options[i].howUnlocked = Random.Range(1,4);
            }
            if(options[i].number == 9){
                options[i].number = Random.Range(1,5);
                options[i].diagonal = true;
            }
            //SCREAM OPTIONS AAAH
            options[i].colorNumber = nextPiece.color;//Random.Range(0,numColors);
            
            /*if(Random.value < 0.25f){
                options[i].locked = true;
            }
            if(Random.value < 0.15f){
                options[i].diagonal = true;
            }
            if(Random.value < 0.15f){
                options[i].number = 0;
            }*/
        }
        if(inTutorial && tutorialStage == (int)TutorialStage.Popping){
            options[1].number = 2;
            options[1].colorNumber = 0;
        }
        if(inTutorial && tutorialStage == (int)TutorialStage.Options){
            options[1].number = 4;
            options[1].colorNumber = 0;
            options[0].number = 5;
            options[0].colorNumber = 0;
            options[2].number = 6;
            options[2].colorNumber = 0;
        }
        if(inTutorial && tutorialStage == (int)TutorialStage.Practice){
            options[1].number = 4;
            options[1].colorNumber = 0;
            options[0].number = 3;
            options[0].colorNumber = 0;
            options[2].number = 1;
            options[2].colorNumber = 0;
        }
        if(inTutorial && tutorialStage == (int)TutorialStage.Cracking){
            options[1].number = 1;
            options[1].colorNumber = 1;
            options[0].number = 1;
            options[0].colorNumber = 1;
            options[2].number = 4;
            options[2].colorNumber = 1;
        }
    }
    Vector3 OptionPlace(int num){
        Vector2Int pos = new Vector2Int((gridSize.x/2)-(numOptions/2),-1);
        pos.x += num;
        return (Vector3)(Vector2)pos+new Vector3(0f,-0.05f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            Services.Visuals.current = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            Services.Visuals.current = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            Services.Visuals.current = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            Services.Visuals.current = 3;
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            Services.Visuals.current = 4;
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            Services.Visuals.current = 5;
        }
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            Services.Visuals.current = 6;
        }
        if(watchingAd){
            if(Time.time > adTimeOver){
                watchingAd = false;
                bigPoints+=adBonus;
                PlayerPrefs.SetInt("Points",bigPoints);
                PlayerPrefs.Save();
            }
            return;
        }
        pointsDisplay.text = bigPoints.ToString();
        pointsDisplay.color = black;
        addBonusDisplay.color = black;
        undoCostDisplay.color = black;
        restartCostDisplay.color = black;
        //TUTORIAL
        if(waitForPopping == false){
            foreach(Rectangle _turn in numOptionsDisplay){
                if(inTutorial && tutorialStage < (int)TutorialStage.Options){
                    _turn.enabled = false;
                }else{
                    _turn.enabled = true;
                }
            }
            if(inTutorial && tutorialStage < (int)TutorialStage.Options){
                options[0].gameObject.SetActive(false);
                options[2].gameObject.SetActive(false);
            }else{
                if(options[0] != null){
                    options[0].gameObject.SetActive(true);
                }
                if(options[2] != null){
                    options[2].gameObject.SetActive(true);
                }
            }
        }
        
        //END TUTORIAL
        highScoreDisplay.text = highScore.ToString();
        if(highScore == 0){
            highScoreDisplay.text = "";
        }
        if(Input.GetKeyDown(KeyCode.S)){
            Debug.Log(SaveBoardToString());
        }
        /*if(Input.GetKeyDown(KeyCode.F)){
            string s = ConvertBoardToString();
            LoadBoardFromString(s);
        }*/
        if(Input.GetKeyDown(KeyCode.Z)){
            Undo();
        }
        if(gameOver == false){
            timePlayed+=Time.deltaTime;
        }
        int _time = Mathf.FloorToInt(timePlayed);
        string sTime = "";
        int seconds = _time%60;
        int minutes =( _time/60)%(60*60);
        int hours = _time/(60*60);
        sTime = seconds.ToString();
        if(seconds < 10){
            sTime = "0"+sTime;
        }
        sTime = ":"+sTime;
        sTime = minutes.ToString()+sTime;
        if(minutes < 10){
            sTime = "0"+sTime;
        }
        if(hours > 0){
            sTime = hours.ToString()+sTime;
            if(hours < 10){
                sTime = "0"+sTime;
            }
        }
        timer.text = sTime;
        timer.color = black;
        if(waitForPopping){
            if(Time.time > lastPop+(beforeCheck ? 0.3f : 0.66f)){
                beforeCheck = false;
                GridChanged();
            }
        }
        else{
            if(earnedPoints.Count > 0){
                int beforePoints = points;
                for(int i = 0; i < earnedPoints.Count;i++){
                    points+=(earnedPoints[i]*(i+1));
                }
                earnedPoints.Clear();
                if(level < levelUnlocks.Count && points >= levelUnlocks[level]){
                    level+=1;
                    unlockAppear = true;
                } 
                if(beforePoints%100 > points%100){
                    bigPoints+=1;
                    PlayerPrefs.SetInt("Points",bigPoints);
                    PlayerPrefs.Save();
                }
            }
            if(turnStarted){
                turnStarted = false;
                //REAL END TURN
                
                if(playedSoFar >= howManyToChoose || ((inTutorial && tutorialStage < (int)TutorialStage.Options) && playedSoFar == 1)){
                    if(inTutorial && tutorialStage == (int)TutorialStage.Practice){
                        tutorialStage++;
                        PlayerPrefs.SetInt("tutorial",tutorialStage);
                    }
                    if(inTutorial && tutorialStage == (int)TutorialStage.Options){
                        tutorialStage++;
                        PlayerPrefs.SetInt("tutorial",tutorialStage);
                    }
                    
                    playedSoFar = 0;
                    RemoveOptions();
                    MakeOptions();
                    
                }
                
                if(gameOver == false){
                    PlayerPrefs.SetString("lastBoard",SaveBoardToString());
                    PlayerPrefs.Save();
                }
                
            }
            if(Input.GetKeyDown(KeyCode.R) && bigPoints >= 5){
                Restart();
                
            }
        }
        float lineSpeed = 0.2f;
        if(playedSoFar < 2){
            numOptionsDisplay[0].Width += (numOptionsDisplayWidth-numOptionsDisplay[0].Width)*lineSpeed*(Time.deltaTime/(1f/60f));
        }else{
            numOptionsDisplay[0].Width += (0-numOptionsDisplay[0].Width)*lineSpeed*(Time.deltaTime/(1f/60f));
            if(numOptionsDisplay[0].Width < 0.05f){
                numOptionsDisplay[0].Width = 0f;
            }
        }
        if(playedSoFar < 1 && numOptionsDisplay[0].Width >= numOptionsDisplayWidth*0.95f){
            numOptionsDisplay[1].Width += (numOptionsDisplayWidth-numOptionsDisplay[1].Width)*lineSpeed*(Time.deltaTime/(1f/60f));
        }else{
            numOptionsDisplay[1].Width += (0-numOptionsDisplay[1].Width)*lineSpeed*(Time.deltaTime/(1f/60f));
            if(numOptionsDisplay[1].Width < 0.05f){
                numOptionsDisplay[1].Width = 0f;
            }
        }
        Color lineColor = waitForPopping ? white : black;
        foreach(Rectangle rect in numOptionsDisplay){
            rect.Color = Color.Lerp(rect.Color,lineColor,lineSpeed*(Time.deltaTime/(1f/60f)));
        }
        /*if(waitForPopping){
            numOptionsLine.Color = white;
        }else{
            numOptionsLine.Color = black;
        }
        numOptionsLine.DashSize = howManyToChoose-playedSoFar;
        float percent = (float)(howManyToChoose-playedSoFar)/(float)howManyToChoose;
        percent*=gridSize.x;
        numOptionsLine.End = new Vector3(percent,-1.5f,0f);*/
        score.text = "";
        string _number = points.ToString("N0");
        for(int i = 0; i < _number.Length;i++){
            if(i < _number.Length-1 && _number[i+1] == ','){
                score.text+="<cspace=-0.2em>";
            }
            score.text += _number[i];
            if(i > 0 && _number[i-1] == ','){
                score.text+="</cspace>";
            }
        } 
        score.color = black;
        highScoreDisplay.color = black;
        if(unlockAppear){
            score.text = score.text+"\n"+levelUnlockDesc[level-1];
        }
        if(inTutorial){
            //score.text = "<size=4>"+tutorialTexts[tutorialStage]+"</size><line-height=110%>\n"+score.text;
            string _tut_text = tutorialTexts[tutorialStage];
            _tut_text = _tut_text.Replace('|','\n');
            score.text += "<line-height=75%>\n<size=3.5>"+_tut_text+"</size><line-height=25%>\n ";
        }else{
            if(earnedPoints.Count > 0){
                //score.text+="\n";
                int totalEarnedPoints = 0;
                for(int i = 0; i < earnedPoints.Count;i++){
                    totalEarnedPoints+=(earnedPoints[i]*(i+1));
                    //score.text+="+("+earnedPoints[i].ToString()+"x"+(i+1).ToString()+")";
                }
                score.text+="+"+totalEarnedPoints;
            }
            if(waitForPopping){
                //score.text+="\n";
                score.text+="<cspace=-0.3em>...</cspace>";
            }
        }
        
        if(gameOver){
            score.text= "Game Over :(\n"+score.text;
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mouseGridPos = new Vector2Int(Mathf.FloorToInt(mousePos.x+0.5f),Mathf.FloorToInt(mousePos.y+0.5f));
        if(waitForPopping == false && gameOver == false){
            if(objectHeld){
                if(Input.GetMouseButtonDown(0)){
                    if(mouseGridPos.x < 0 || mouseGridPos.y < 0 || mouseGridPos.x >= gridSize.x || mouseGridPos.y >= gridSize.y){
                        if(inTutorial && tutorialStage == (int)TutorialStage.Placing){
                            tutorialStage--;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        heldObject.isSelected = false;
                        objectHeld = false;
                        heldObject.depth = 10;
                        heldObject = null;
                    }else if(grid.ContainsKey(mouseGridPos)){
                        if(inTutorial && tutorialStage == (int)TutorialStage.Placing){
                            tutorialStage--;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        heldObject.isSelected = false;
                        objectHeld = false;
                        heldObject.depth = 10;
                        heldObject = null;
                    }else{
                        if(inTutorial && tutorialStage == (int)TutorialStage.Placing){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        
                        if(inTutorial && tutorialStage == (int)TutorialStage.GameOver){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        if(inTutorial && tutorialStage == (int)TutorialStage.Combo){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        if(inTutorial && tutorialStage == (int)TutorialStage.Undo){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        Services.AudioManager.PlaySound(Services.AudioManager.select);
                        turns.Add(SaveBoardToString());

                        heldObject.transform.position = (Vector2)mouseGridPos;
                        heldObject.isSelected = false;
                        heldObject.isOption = false;
                        heldObject.depth = 0;
                        AddToGrid(mouseGridPos,heldObject);
                        objectHeld = false;
                        heldObject = null;
                        playedSoFar++;
                        
                    }
                    
                }else if(Input.GetMouseButtonUp(0)){
                    if(mouseGridPos.x < 0 || mouseGridPos.y < 0 || mouseGridPos.x >= gridSize.x || mouseGridPos.y >= gridSize.y){
                    }else if(grid.ContainsKey(mouseGridPos)){
                    }else{
                        if(inTutorial && tutorialStage == (int)TutorialStage.Placing){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        if(inTutorial && tutorialStage == (int)TutorialStage.GameOver){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        if(inTutorial && tutorialStage == (int)TutorialStage.Combo){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        if(inTutorial && tutorialStage == (int)TutorialStage.Undo){
                            tutorialStage++;
                            PlayerPrefs.SetInt("tutorial",tutorialStage);
                        }
                        Services.AudioManager.PlaySound(Services.AudioManager.select);
                        turns.Add(SaveBoardToString());
                        heldObject.transform.position = (Vector2)mouseGridPos;
                        heldObject.isSelected = false;
                        heldObject.isOption = false;
                        heldObject.depth = 0;
                        AddToGrid(mouseGridPos,heldObject);
                        objectHeld = false;
                        heldObject = null;
                        playedSoFar++;
                    }
                }
            }else{
                foreach(Object obj in options){
                    if(obj == null){
                        continue;
                    }
                    if(obj.isOption == false){
                        continue;
                    }
                    if(obj.collider.OverlapPoint(mousePos)){
                        if(Input.GetMouseButtonDown(0)){
                            if(inTutorial && tutorialStage == (int)TutorialStage.Grabing){
                                tutorialStage++;
                                PlayerPrefs.SetInt("tutorial",tutorialStage);
                            }
                            obj.isSelected = true;
                            Services.AudioManager.PlaySound(Services.AudioManager.select);
                            objectHeld = true;
                            heldObject = obj;
                            heldObject.depth = 20;
                            break;
                        }
                    }
                }
            }
        }
        
        
        for(var i = 0; i < options.Count;i++){
            Object o = options[i];
            if(o == null){
                continue;
            }
            if(o.isOption == false){
                continue;
            }
            if(o.isSelected){
                Vector3 targetPosition = (Vector2)mouseGridPos;
                targetPosition += Vector3.right*Mathf.Sin(Time.time*5f)*0.02f;
                targetPosition+=Vector3.up*0.1f;
                if(grid.ContainsKey(mouseGridPos)){
                    targetPosition+=Vector3.up*0.45f;
                    o.transform.localScale += ((Vector3.one*0.95f)-o.transform.localScale)*0.5f*(Time.deltaTime/(1f/60f));
                }else{
                    o.transform.localScale += ((Vector3.one*1f)-o.transform.localScale)*0.5f*(Time.deltaTime/(1f/60f));
                }
                o.transform.position += (targetPosition-o.transform.position)*0.25f*(Time.deltaTime/(1f/60f));
                
                continue;
            }
            o.transform.position = OptionPlace(i);
        }
    }
    public void Undo(){
        if(waitForPopping == false && turns.Count >= 1 && gameOver == false){
            if(inTutorial && tutorialStage == (int)TutorialStage.Undo){
                tutorialStage++;
                PlayerPrefs.SetInt("tutorial",tutorialStage);
            }
            bigPoints-=1;
            PlayerPrefs.SetInt("Points",bigPoints);
            string lastTurn = turns[turns.Count-1];
            turns.RemoveAt(turns.Count-1);
            Debug.Log(lastTurn);
            LoadBoardFromString(lastTurn);
            PlayerPrefs.SetString("lastBoard",lastTurn);
            PlayerPrefs.Save();
        }
    }
    public void Restart(){
        if(waitForPopping == false){
            bigPoints-=5;
            PlayerPrefs.SetInt("Points",bigPoints);
            PlayerPrefs.Save();
            tetrisBag.Clear();
            turns.Clear();
            PlayerPrefs.DeleteKey("lastBoard");
            points = 0;
            timePlayed = 0;
            gameOver = false;
            playedSoFar = 0;
            level = 0;
            ClearBoard();
            RemoveOptions();
            MakeOptions();
        }
    }
    void ClearBoard(){
        
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach(Vector2Int pos in grid.Keys){
            positions.Add(pos);
        }
        foreach(Vector2Int pos in positions){
            RemoveObject(grid[pos]);
        }
    }
    void AddToGrid(Vector2Int pos, Object o, bool chains = true){
        

        unlockAppear = false;
        o.position = pos;
        o.depth = 0;
        grid.Add(pos,o);
        if(chains){
            earnedPoints.Clear();
            turnStarted = true;
            lastPop = Time.time;
            waitForPopping = true;
            beforeCheck = true;
            //GridChanged();
        }
        
    }
    void AddToGrid(Vector2Int pos, Object o){
        AddToGrid(pos,o,true);
    }
    void GridChanged(){
        int pointsThisLevel = 0;
        bool didGridChange = false;
        List<Vector2Int> toBeDeleted = new List<Vector2Int>();
        foreach(Vector2Int pos in grid.Keys){
            same_number.Clear();
            same_color.Clear();
            CheckGridForSameColor(pos);
            if(same_color.Count > 0){
                for(int i = 0; i < 1; i++){
                    Vector2Int _pos = same_color[0];
                //foreach(Vector2Int _pos in same_color){
                    if(toBeDeleted.Contains(_pos)){
                        continue;
                    }
                    Object obj = grid[_pos];
                    if(same_color.Count != obj.number){
                        if(obj.number == 0){
                            bool popNoNumber = false;
                            foreach(Vector2Int __pos in same_color){
                                if(same_color.Count == grid[__pos].number){
                                    popNoNumber = true;
                                    break;
                                }
                            }
                            if(popNoNumber == false){
                                continue;
                            }
                        }else{
                            continue;
                        }
                        
                    }
                    didGridChange = true;
                    int worth = 1;
                    if(obj.locked || obj.number == 0 || obj.diagonal){
                        worth+=1;
                    }
                    pointsThisLevel+=worth;//obj.number;
                    toBeDeleted.Add(_pos);
                    GameObject popObj = GameObject.Instantiate(scorePopUpPrefab,grid[_pos].transform.position,Quaternion.identity,transform);
                    popObj.GetComponent<ScorePopUp>().number = worth;//obj.number;
                    popObj.GetComponent<ScorePopUp>().multiplier = (earnedPoints.Count+1);
                    //sum+=next;
                    /*obj.number-=1;
                    if(obj.number < 3){
                        RemoveObject(obj);
                        if(obj == o){
                            removed = true;
                        }
                    }*/
                }
                
            }
        }
        foreach(Vector2Int pos in toBeDeleted){
            Vector2Int[] _directions = (grid[pos].diagonal ? diagonalDirections : directions);
            for(int i = 0; i < _directions.Length;i++){
                Vector2Int dir = _directions[i];
                Vector2Int n_pos = pos+dir;
                if(grid.ContainsKey(n_pos) == false){
                    continue;
                }
                if(toBeDeleted.Contains(n_pos)){
                    continue;
                }
                if((grid[pos].colorNumber == grid[n_pos].colorNumber || (grid[n_pos].number == 0 || grid[n_pos].number == 1)) && grid[n_pos].locked == false){
                    continue;
                }
                if(grid[pos].colorNumber == grid[n_pos].colorNumber && grid[n_pos].locked && grid[n_pos].CanConnectToLocked(i)){
                    continue;
                }
                //SCREAMSCRAEM
                if(inTutorial && tutorialStage == (int)TutorialStage.Cracking){
                    tutorialStage++;
                    PlayerPrefs.SetInt("tutorial",tutorialStage);
                }
                GameObject crackObj = GameObject.Instantiate(crackShotPrefab,(Vector2)pos,Quaternion.identity,transform);
                CrackShot crackShot = crackObj.GetComponent<CrackShot>();
                crackShot.target = n_pos;
                crackShot.color = colors[grid[pos].colorNumber];
                if(dir.y != 0){
                    crackObj.transform.localEulerAngles = new Vector3(0,0,90f);
                }
                bool justLocked = grid[n_pos].locked;
                if(grid[n_pos].locked && grid[n_pos].CanConnectToLocked(i) == false){
                    grid[n_pos].lockDirection = -1;
                }else{
                    grid[n_pos].number-=1;
                }
                if(justLocked == false && grid[n_pos].number < 1){
                    RemoveObject(grid[n_pos]);
                }else{
                    Boop boopcheck = grid[n_pos].GetComponent<Boop>();
                    if(ReferenceEquals(boopcheck,null) == false){

                    }else{
                        grid[n_pos].gameObject.AddComponent<Boop>();
                        grid[n_pos].gameObject.GetComponent<Boop>().depth = earnedPoints.Count;
                        if(grid[n_pos].lockDirection < 0){
                            grid[n_pos].gameObject.GetComponent<Boop>().unlocking = true;
                        }
                        
                    }
                }
            }
            Services.AudioManager.PlaySound(Services.AudioManager.pop,earnedPoints.Count);
            Boop boop = grid[pos].gameObject.GetComponent<Boop>();
            if(ReferenceEquals(boop,null)== false){
                Destroy(boop);
            }
            grid[pos].gameObject.AddComponent<WinAndDestroy>();
            grid[pos].isPopping = true;
            grid[pos].gameObject.GetComponent<WinAndDestroy>().depth = earnedPoints.Count;
            RemoveObject(grid[pos]);
            if(inTutorial && tutorialStage == (int)TutorialStage.Popping){
                tutorialStage++;
                PlayerPrefs.SetInt("tutorial",tutorialStage);
                playedSoFar = 0;
                RemoveOptions();
                MakeOptions();
            }
        }
        if(pointsThisLevel > 0){
            if(grid.Count == 0){
                Debug.Log("GRID CLEAR");
                Vector2Int _pos = new Vector2Int(1,1);
                GameObject popObj = GameObject.Instantiate(scorePopUpPrefab,(Vector2)_pos,Quaternion.identity,transform);
                popObj.GetComponent<ScorePopUp>().number = -1;
                popObj.GetComponent<ScorePopUp>().multiplier = (earnedPoints.Count+1);
                pointsThisLevel+=9;
            }
            earnedPoints.Add(pointsThisLevel);
        }
        
        if(didGridChange){
            
            lastPop = Time.time;
            waitForPopping = true;
        }else{
            waitForPopping = false;
            
            //TURN OVER
            if(grid.Count == (gridSize.x*gridSize.y)){
                gameOver = true;
                PlayerPrefs.DeleteKey("lastBoard");
                if(points > highScore){
                    highScore = points;
                    PlayerPrefs.SetInt("highScore",highScore);
                    PlayerPrefs.Save();
                }
            }
        }
        
    }
    void RemoveObject(Object o){
        grid.Remove(o.position);
        Boop boop = o.gameObject.GetComponent<Boop>();
        if(ReferenceEquals(boop,null)== false){
            Destroy(boop);
        }
        WinAndDestroy win = o.gameObject.GetComponent<WinAndDestroy>();
        if(ReferenceEquals(win,null)== false){
            return;
        }
        o.gameObject.AddComponent<ShrinkAndDestroy>();
        //GameObject.Destroy(o.gameObject);
    }
    void CheckGridForSameNumber(Vector2Int pos){
        same_number.Add(grid[pos]);
        int number = grid[pos].number;
        for(var i = 0; i < 4; i++){
            Vector2Int n_pos = pos+directions[i];
            if(grid.ContainsKey(n_pos)){
                if(same_number.Contains(grid[n_pos])){
                    continue;
                }
                if(grid[n_pos].number == number){
                    CheckGridForSameNumber(n_pos);
                }
            }
        }
    }
    void CheckGridForSameColor(Vector2Int pos){
        same_color.Add(pos);
        int color_number = grid[pos].colorNumber;
        Vector2Int[] _directions = (grid[pos].diagonal ? diagonalDirections : directions);
        for(var i = 0; i < 4; i++){
            if(grid[pos].CanConnectOut(i) == false){
                continue;
            }
            Vector2Int n_pos = pos+_directions[i];
            switch(i){
                case 0:
                    if(tiles[pos].upWall){
                        continue;
                    }
                    break;
                case 1:
                    if(tiles[pos].rightWall){
                        continue;
                    }
                    break;
                case 2:
                    if(tiles[pos].downWall){
                        continue;
                    }
                    break;
                case 3:
                    if(tiles[pos].leftWall){
                        continue;
                    }
                    break;
            }
            if(grid.ContainsKey(n_pos)){
                if(grid[n_pos].CanConnectToLocked(i) == false){
                    continue;
                }
                if(same_color.Contains(n_pos)){
                    continue;
                }
                if(grid[n_pos].colorNumber == color_number){
                    CheckGridForSameColor(n_pos);
                }
            }
        }
    }
    string SaveBoardToString(){
        var s = "";
        s+=points.ToString()+'\n';
        for(var y = 0; y < gridSize.y;y++){
            for(var x = 0; x < gridSize.x;x++){
                if(x > 0){
                    s+="-";
                }
                Vector2Int pos = new Vector2Int(x,y);
                if(grid.ContainsKey(pos) == false){
                    s+="__";
                    continue;
                }
                s+=grid[pos].colorNumber.ToString()+grid[pos].number.ToString()+(grid[pos].locked ? grid[pos].lockDirection : 5).ToString()+grid[pos].howUnlocked.ToString()+(grid[pos].diagonal ? "1" : "0");
            }
            s+="\n";
        }
        s+="***\n";
        for(var i = 0; i < options.Count;i++){
            if(i > 0){
                s+="-";
            }
            if(options[i] == null){
                s+="__";
                continue;
            }
            if(options[i].isOption){
                s+=options[i].colorNumber.ToString()+options[i].number.ToString()+(options[i].locked ? options[i].lockDirection : 5).ToString()+options[i].howUnlocked.ToString()+(options[i].diagonal ? "1" : "0");
            }else{
                s+="__";
            }
        }
        s+="\nend\n";
        for(int i = 0; i < tetrisBag.Count;i++){
            if(i > 0){
                s+="-";
            }
            Piece p = tetrisBag[i];
            s+=tetrisBag[i].color.ToString()+tetrisBag[i].number.ToString();
        }
        s+="\n"+playedSoFar.ToString();
        s+="\n"+timePlayed.ToString();
        s+="\n"+level.ToString();
        return s;
        
    }
    void LoadBoardFromString(string _string){
        ClearBoard();
        RemoveOptions();
        string[] lines = _string.Split('\n');
        points = int.Parse(lines[0]);
        bool is_options = false;
        int bagLineIndex = -1;
        for(var y = 1; y < lines.Length;y++){
            if(lines[y][0] == 'e'){
                bagLineIndex = y+1;
                break;
            }
            if(lines[y][0] == '*'){
                is_options = true;
                continue;
            }
            string[] pieces = lines[y].Split('-');
            for(var x = 0; x < pieces.Length;x++){
                if(pieces[x] == "__"){
                    if(is_options){
                        options.Add(null);
                    }
                    continue;
                }
                int color = int.Parse(pieces[x][0].ToString());
                int number = int.Parse(pieces[x][1].ToString());
                int locked = int.Parse(pieces[x][2].ToString());
                int howUnlocked = int.Parse(pieces[x][3].ToString());
                int diagonal = int.Parse(pieces[x][4].ToString());
                Vector2Int pos = new Vector2Int(x,y-1);
                GameObject obj = GameObject.Instantiate(objPrefab,transform.position,Quaternion.identity,transform);
                Object o = obj.GetComponent<Object>();
                obj.AddComponent<ExpandAndRemove>();
                o.number = number;//Random.Range(generatorRange.x,generatorRange.y+1);
                o.colorNumber = color;//Random.Range(0,numColors);
                o.lockDirection = (locked == 5 ? -1 : locked);
                o.howUnlocked = howUnlocked;
                o.diagonal = (diagonal == 0 ? false : true);
                
                if(is_options){
                    o.transform.position = OptionPlace(x);
                    options.Add(obj.GetComponent<Object>());
                    o.isOption = true;
                    o.depth = 10;
                }else{
                    
                    o.transform.position = (Vector2)pos;
                    AddToGrid(pos,o,false);
                }
                
            }
        }
        string[] bagLine = lines[bagLineIndex].Split('-');
        tetrisBag.Clear();
        foreach(string _piece in bagLine){
            if(_piece.Length < 1){
                break;
            }
            int color = int.Parse(_piece[0].ToString());
            int number = int.Parse(_piece[1].ToString());
            tetrisBag.Add(new Piece(number,color));
        }
        playedSoFar = int.Parse(lines[bagLineIndex+1]);
        timePlayed = float.Parse(lines[bagLineIndex+2]);
        level = int.Parse(lines[bagLineIndex+3]);
    }
    public int OppositeDirection(int _dir){
        return (_dir+2+4)%4;
    }
    
}
