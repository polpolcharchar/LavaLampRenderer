using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCoolingScript : MonoBehaviour
{

    private Rigidbody2D rb;

    public float coolingRate = 0.002f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        //get the scale of the object this is attached to:
        //float scale = transform.localScale.y;

        //get the scale of the parent object:
        float scale = transform.parent.localScale.y;


        rb.gravityScale += coolingRate * (scale / 10) * 0.3f;


        if(rb.gravityScale >= 0.1f * (scale / 10) * 0.3f)
        {
            rb.gravityScale = 0.1f * (scale / 10) * 0.3f;
        }
    }
}
