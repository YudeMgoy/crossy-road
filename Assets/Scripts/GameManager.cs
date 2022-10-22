using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int minZPos = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    int backDistance = -5;
    public PlayerControl player;
    public GameObject gameOverPanel;
    public TMP_Text lastScoreText;

    //int maxZpos;
    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);
    private void Start()
    {
        // belakang
        for (int z = minZPos; z <= 0; z++) {
            CreateTerrain(grass, z);
        }
        // depan
        for (int z = 1; z < frontDistance; z++)
        {
            var prefab = GetNextRandomTerrainPrefab(z);
            // instantiate blocknya
            CreateTerrain(prefab, z);
        }

        player.Setup(backDistance, extent);
    }

    private void CreateTerrain(GameObject prefab, int zPos)
    {
        var go = Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        var tb = go.GetComponent<TerrainBlock>();
        tb.Build(extent);
        map.Add(zPos, tb);

        Debug.Log(map[zPos] is Road);
    }

    private GameObject GetNextRandomTerrainPrefab(int nextPos)
    {
        bool isUniform = true;
        var tbRef = map[nextPos - 1];
        for (int distance = 2; distance <= maxSameTerrainRepeat; distance++)
{
            if (map[nextPos - distance].GetType() != tbRef.GetType())
            {
                isUniform = false;
                break;
            }
        }
        if (isUniform)
        {
            if (tbRef is Grass)
                return road;            
            else
                return grass;
        }

        // penentuan terrain block dengan probabilitas 50%
        return Random.value> 0.5f? road : grass;
    }

    public void GameOver()
    {
        player.enabled = false;
        gameOverPanel.SetActive(true);
        lastScoreText.text = "Last Score: " + player.MaxTravel.ToString();
    }

    int playerLastMaxTravel;

    private void Update()
    {
        if (player.MaxTravel == playerLastMaxTravel)
            return;

        playerLastMaxTravel = player.MaxTravel;

        //var randPrefab = GetNextRandomTerrainPrefab(player.MaxTravel + frontDistance);
        //CreateTerrain(randPrefab, player.MaxTravel + frontDistance);

        //var lastTb = map[player.MaxTravel-1 + backDistance];
        //map.Remove(player.MaxTravel - 1 + backDistance);
        //Destroy(lastTb);

        //player.Setup(player.MaxTravel + backDistance, extent);
    }
}
