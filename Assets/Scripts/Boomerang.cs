using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour {
    private bool _initialized = false;
    private Transform hex;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private Material[] mats;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        mats = new Material[transform.childCount];
        for(int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    void FixedUpdate ()
    {
        if (_initialized)
        {
            float distance = Vector2.Distance(transform.position, hex.position); //distance to the HEX
            if (distance > 20f) distance = 20f; //max out distance to 20 
            foreach (Material t in mats)
                t.color = new Color((1f - (distance / 20f)), 0, distance / 20f); //close is red, far is blue

            float[] inputs = new float[1];

            float angle = transform.eulerAngles.z % 360f; // Current boomerang heading
            if (angle < 0f)
                angle += 360f;

            Vector2 deltaVector = (hex.position - transform.position).normalized;
   
            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x) * Mathf.Rad2Deg; //angle between x axis and vector towards HEX

            rad = rad % 360;
            if (rad < 0)
            {
                rad = 360 + rad;
            }

            rad = 90f - rad;
            if (rad < 0f)
            {
                rad += 360f;
            }
            rad = 360 - rad;

            rad -= angle;    //calculate relative heading.

            if (rad < 0)
                rad = 360 + rad;
            if (rad >= 180f)  // do we need to turn CW+ or CCW-
                rad = 360 - rad * -1f;
                      
            rad *= Mathf.Deg2Rad;

            inputs[0] = rad / (Mathf.PI);


            float[] output = net.FeedForward(inputs);

            rBody.velocity = 2.5f * transform.up;
            rBody.angularVelocity = 500f * output[0];

            net.AddFitness((1f-Mathf.Abs(inputs[0])));
        }
	}

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.hex = hex;
        this.net = net;
        _initialized = true;
    }

    
}
