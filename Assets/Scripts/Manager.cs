using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject boomerPrefab;
    public GameObject hex;

    private bool _isTraning = false;
    private int _populationSize = 4;
    private int _generationNumber = 0;
    //private int[] layers = new int[] { 1, 10, 10, 1  }; 
    private List<NeuralNetwork> boomerBrainz;
    private bool leftMouseDown = false;
    private List<Boomerang> boomerangList = null;


    void Timer()
    {
        _isTraning = false;
    }


	void Update ()
    {
        if (_isTraning == false)
        {
            if (_generationNumber == 0)
            {
                InitBoomerangNeuralNetworks();
            }
            else
            {
                boomerBrainz.Sort();
                for (int i = 0; i < _populationSize / 2; i++)
                {
                    boomerBrainz[i] = new NeuralNetwork(boomerBrainz[i+(_populationSize / 2)]);
                    boomerBrainz[i].Mutate();

                    boomerBrainz[i + (_populationSize / 2)] = new NeuralNetwork(boomerBrainz[i + (_populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < _populationSize; i++)
                {
                    boomerBrainz[i].SetFitness(0f);
                }
            }

           
            _generationNumber++;
            
            _isTraning = true;
            Invoke("Timer",15f);
            CreateBoomerangBodies();
        }


        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

	    if (leftMouseDown != true) return;
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
        for (int i = 0; i < _populationSize; i++)
        {
            var boomer = ((GameObject)Instantiate(boomerPrefab)).GetComponent<Boomerang>();
            boomer.Init(boomerBrainz[i],hex.transform);
            boomerangList.Add(boomer);
        }

    }

    /// <summary>
    /// Initializes the Boomerangs' "brains" <br/>
    /// Creates the N Networks boomerBrainz.
    /// </summary>
    void InitBoomerangNeuralNetworks()
    {
        //population must be even
        if (_populationSize % 2 != 0)  _populationSize++;

        boomerBrainz = new List<NeuralNetwork>();       

        for (int i = 0; i < _populationSize; i++)
        {           
            var boomerBrain = new NeuralNetwork(1, 10, 20, 10, 1); //1 input and 1 output
            boomerBrain.Mutate();
            boomerBrainz.Add(boomerBrain);
        }
    }
}
