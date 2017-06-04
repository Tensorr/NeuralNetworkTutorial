using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject boomerPrefab;
    public GameObject hex;

    // these allow for tweaking from Unity frontend
    public int PopulationSize = 4;
    public float TrainTime = 15f;
    [Space(10)]
    public int[] Layers = new int[] { 1, 10, 10, 1 };
    // --

    private List<Boomerang> boomerangList = null;
    private List<NeuralNetwork> boomerBrainz;
    private bool _leftMouseDown = false;
    private int _generationNumber = 0;  
    private bool _isTraning = false;

    void Timer()
    {
        _isTraning = false;
    }


	void Update ()
    {
        if (!_isTraning) // if not training 
        {
            if (_generationNumber == 0)
            {
                InitBoomerangNeuralNetworks();
            }
            else
            {
                boomerBrainz.Sort(); //sort by fit worst to best

                for (int badHalf = 0; badHalf < PopulationSize / 2; badHalf++)
                {
                    var goodHalf = badHalf + (PopulationSize/2);
                    //was this the other way around on purpose? yeah a fit of 1 is perfect -1 is horrible
                    //copy the good half over the bad half and mutate it 
                    boomerBrainz[badHalf] = new NeuralNetwork(boomerBrainz[goodHalf]); 
                    boomerBrainz[badHalf].Mutate();

                    //swapped. keep the good half
                    boomerBrainz[goodHalf] = new NeuralNetwork(boomerBrainz[goodHalf]); //todo: matrix reset vs this ? why would it be better?
                    if (_generationNumber < 5) boomerBrainz[goodHalf].Mutate(); //increase early mutations
                    //reset all their fitnesses to 0f
                    boomerBrainz[badHalf].SetFitness(0f);
                    boomerBrainz[goodHalf].SetFitness(0f); 
                }
            }
           
            _generationNumber++;
            
            _isTraning = true;
            Invoke("Timer",TrainTime); //train for trainTime sec
            CreateBoomerangBodies();
        }


        if (Input.GetMouseButtonDown(0))
        {
            _leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _leftMouseDown = false;
        }

	    if (_leftMouseDown != true) return;
	    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	    hex.transform.position = mousePosition;
    }

    /// <summary>
    /// Destroys the current Boomerangs, if any
    /// Creates _populationSize new ones
    /// </summary>
    private void CreateBoomerangBodies()
    {
        // if we have any, kill em!
        if (boomerangList != null)
        {
            foreach (var t in boomerangList)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
        // Rise my babies!
        boomerangList = new List<Boomerang>();
        for (int i = 0; i < PopulationSize; i++)
        {
            var boomer = ((GameObject)Instantiate(boomerPrefab)).GetComponent<Boomerang>();
            boomer.Init(boomerBrainz[i],hex.transform);
            boomerangList.Add(boomer);
        }

    }

    /// <summary>
    /// Initializes the Boomerangs' "brains" <br/>
    ///
    /// </summary>
    void InitBoomerangNeuralNetworks()
    {
        //population must be even
        if (PopulationSize % 2 != 0) PopulationSize++;

        boomerBrainz = new List<NeuralNetwork>();       

        for (int i = 0; i < PopulationSize; i++)
        {
            //var boomerBrain = new NeuralNetwork(1, 10, 4, 1); //1 input and 1 output
            var boomerBrain = new NeuralNetwork(Layers); // this allows for tweaking from Unity frontend
            boomerBrain.Mutate();
            boomerBrainz.Add(boomerBrain);
        }
    }
}
