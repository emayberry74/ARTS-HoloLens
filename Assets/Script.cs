using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject go;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision C)
    {
        if(C.transform.tag == "player")
        {
            //go.transform.SetParent = C.transform;
        }
    }
}
