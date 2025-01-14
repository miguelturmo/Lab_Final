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
    void Start()
    {
        allFletchings = new GameObject[numFletchings];
        allTalonflames = new List<GameObject>();
        for (int i = 0; i < numFletchings; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-limits.x, limits.x),
                Random.Range(-limits.y, limits.y), 
                Random.Range(-limits.z, limits.z));
            allFletchings[i] = Instantiate(fletchingPrefab, pos, Quaternion.identity);
        }

        for (int i = 0; i < numTalonflames; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-limits.x, limits.x),
                Random.Range(-limits.y, limits.y),
                Random.Range(-limits.z, limits.z));
            GameObject talonflame = Instantiate(talonflamePrefab, pos, Quaternion.identity);
            allTalonflames.Add(talonflame); // Add to the leaders list
        }

        FM = this;

        goalPos = this.transform.position;
    }

    // Update is called once per frame
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
                goalPos = this.transform.position + new Vector3(
                    Random.Range(-limits.x, limits.x),
                    Random.Range(-limits.y, limits.y),  
                    Random.Range(-limits.z, limits.z));
            }
        }
    }
}