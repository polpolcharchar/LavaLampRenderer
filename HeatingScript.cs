using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatingScript : MonoBehaviour
{

    public float heatingRate = 0.01f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            BallCollided(collision);
        }
    }

    private void BallCollided(Collider2D ballCollider)
    {

        float scale = transform.parent.localScale.y;


        Rigidbody2D ballRigidbody = ballCollider.GetComponent<Rigidbody2D>();
        if (ballRigidbody != null)
        {
            //decrease the gravity scale of the ball
            ballRigidbody.gravityScale -= heatingRate * (scale / 10) * 0.3f;

        }
    }
}
