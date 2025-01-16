using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public GameObject treasure;
    public float respawnTime = 10.0f;

    private float timer = 0.0f;

    void Update()
    {
        if (!treasure.activeInHierarchy)
        {
            timer += Time.deltaTime;

            if (timer >= respawnTime)
            {
                treasure.SetActive(true);
                timer = 0.0f;
            }
        }
    }
}

