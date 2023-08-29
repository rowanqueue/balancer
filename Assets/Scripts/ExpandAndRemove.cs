using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandAndRemove : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale+=(Vector3.one-transform.localScale)*0.15f*Time.deltaTime/(1f/60f);
        if(transform.localScale.x > 0.98f){
            transform.localScale = Vector3.one;
            Destroy(this);
        }
    }
}
