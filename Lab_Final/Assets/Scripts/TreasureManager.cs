using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public GameObject treasure;
    public float respawnTime = 10.0f;

    private bool isStolen = false;
    private float timer = 0.0f;

    void Update()
    {
        if (!treasure.activeInHierarchy && !isStolen)
        {
            isStolen = true;
            //Debug.Log("Treasure stolen");
        }

        if (isStolen)
        {
            timer += Time.deltaTime;

            if (timer >= respawnTime)
            {
                treasure.SetActive(true);
                isStolen = false;
                timer = 0.0f;
                //Debug.Log("Treasure Respawned");
            }
        }
    }
}

