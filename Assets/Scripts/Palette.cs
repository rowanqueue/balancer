using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Palette/List", order = 1)]
public class Palette : ScriptableObject {
    public string objectName = "palette";
    public Color backgroundColor = new Color(1f,1f,1f,1f);//subtle background color
    public Color gridColor = new Color(1f,1f,1f,1f);//liiines
    public Color firstColor = new Color(1f,1f,1f,1f);//shpae coloe
    public Color secondColor = new Color(1f,1f,1f,1f);//shpae coloe
    public Color thirdColor = new Color(1f,1f,1f,1f);//shpae coloe
    public Color textColor = new Color(1f,1f,1f,1f);//teext
}