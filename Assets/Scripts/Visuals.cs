using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visuals : MonoBehaviour
{
    public int current;
    public List<Palette> palettes;
    public Palette palette => palettes[current];
    // Start is called before the first frame update
    public void Initialize()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.backgroundColor = palette.backgroundColor;
        Services.GameController.colors[0] = palette.firstColor;
        Services.GameController.colors[1] = palette.secondColor;
        Services.GameController.colors[2] = palette.thirdColor;
        Services.GameController.black = palette.gridColor;
        Services.GameController.white = palette.textColor;
    }
}
