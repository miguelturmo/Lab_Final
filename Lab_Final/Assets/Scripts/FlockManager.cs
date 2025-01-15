using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM; 
    public GameObject fletchingPrefab;
    public GameObject talonflamePrefab;
    public int numFletchings = 20;
    public int numTalonflames = 1;
    public GameObject[] allFletchings;
    public List<GameObject> allTalonflames;
    public Vector3 limits = new Vector3(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header("Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    [Header("Leader Influence")]
    [Range(0, 100)]
    public int leaderInfluence;

    // Start is called before the first frame update
    // This method initializes the pokemons by spawning them within the defined limits
    void Start()
    {
        // Initialize the array to hold all the pokemons
        allFletchings = new GameObject[numFletchings];
        allTalonflames = new List<GameObject>();

        // Loop to create and place each pokemon randomly within the limits
        for (int i = 0; i < numFletchings; i++)
        {
            // Calculate a random position within the limits
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-limits.x, limits.x),
                Random.Range(-limits.y, limits.y), 
                Random.Range(-limits.z, limits.z));
            // Instantiate the fletching prefab at the random position with no rotation
            allFletchings[i] = Instantiate(fletchingPrefab, pos, Quaternion.identity);
        }

        for (int i = 0; i < numTalonflames; i++)
        {
            // Calculate a random position within the limits
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-limits.x, limits.x),
                Random.Range(-limits.y, limits.y),
                Random.Range(-limits.z, limits.z));
            // Instantiate the talonflame prefab at the random position with no rotation
            GameObject talonflame = Instantiate(talonflamePrefab, pos, Quaternion.identity);
            allTalonflames.Add(talonflame); // Add to the leaders list
        }

        // Set the static reference to this instance of FlockManager
        FM = this;

        // Initialize the goal position as the FlockManager's position
        goalPos = this.transform.position;
    }

    // Update is called once per frame
    // Randomly updates the goal position for the pokemons to fly towards
    void Update()
    {
        if (Random.Range(0, 100) < leaderInfluence)
        {
            if (allTalonflames.Count > 0)
            {
                // Choose a random leader's position as the new goal
                int talonflamesIndex = Random.Range(0, allTalonflames.Count);
                goalPos = allTalonflames[talonflamesIndex].transform.position;
            }
            else
            {
                // Calculate a new random goal position within the limits
                goalPos = this.transform.position + new Vector3(
                    Random.Range(-limits.x, limits.x),
                    Random.Range(-limits.y, limits.y),  
                    Random.Range(-limits.z, limits.z));
            }
        }
    }
}