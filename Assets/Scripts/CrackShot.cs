using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class CrackShot : MonoBehaviour
{
    public bool atTarget;
    public Vector2 target;
    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rectangle>().Color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if(atTarget == false){
            transform.position += ((Vector3)target-transform.position)*0.15f*(Time.deltaTime/(1f/60f));
            if(Vector2.Distance(target,(Vector2)transform.localPosition) < 0.05f){
                atTarget = true;
                
            }
        }else{
            transform.localScale += (Vector3.zero-transform.localScale)*0.1f*(Time.deltaTime/(1f/60f));
            if(transform.localScale.x < 0.02f){
                Destroy(this.gameObject);
            }
        }
        
    }
}
