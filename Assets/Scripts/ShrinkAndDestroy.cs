using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    float speed = 0.15f;
    // Start is called before the first frame update
    void Awake()
    {
        ExpandAndRemove ear = GetComponent<ExpandAndRemove>();
        if(ear != null){
            Destroy(ear);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentScale = transform.localScale.x;
        currentScale+=(0-currentScale)*speed*Time.deltaTime/(1f/60f);
        if(currentScale < 0.01f){
            GameObject.Destroy(gameObject);
        }
        transform.localScale = Vector3.one*currentScale;
    }
}
