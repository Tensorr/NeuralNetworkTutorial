using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject boomerPrefab;
    public GameObject hex;

    // these allows for tweaking from Unity frontend
    public int PopulationSize = 4;
    public const float TrainTime = 15f;
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
                boomerBrainz.Sort();
                for (int i = 0; i < PopulationSize / 2; i++)
                {
                    //copy the 2nd half over the first half and mutate it 
                    boomerBrainz[i] = new NeuralNetwork(boomerBrainz[i+(PopulationSize / 2)]); 
                    boomerBrainz[i].Mutate();

                    //todo: keep the second half as is? trace it.
                    boomerBrainz[i + (PopulationSize / 2)] = new NeuralNetwork(boomerBrainz[i + (PopulationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol

                    //reset all their fitnesses to 0f
                    boomerBrainz[i].SetFitness(0f);
                    boomerBrainz[i + (PopulationSize / 2)].SetFitness(0f); // yeah i know, but i prefer this to two 'for' loops.
                }
            }
           
            _generationNumber++;
            
            _isTraning = true;
            Invoke("Timer",TrainTime); //train for _trainTime sec
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
        if (PopulationSize % 2 != 0)  PopulationSize++;

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
