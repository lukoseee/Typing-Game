using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandleController : MonoBehaviour
{   

    private List<Candle> candles = new List<Candle>();

    private int currentCandleIndex = 0;

    public int spacing = 2;
    public float initialX = -8.191f;
    public Candle candlePrefab;
    public Transform candleParent;

    public void lightNextCandle()
    {   
        if(currentCandleIndex >= candles.Count)
                return;

        candles[currentCandleIndex].Light();
        Debug.Log($"Lit candle {currentCandleIndex} of {candles.Count}");

        currentCandleIndex++;
    }

    public void ResetCandles()
    {   
        
        currentCandleIndex = 0;

        for (int i = 0; i < candles.Count; i++)
        {
            candles[i].ResetCandle();
        }
    }

    public void SpawnCandles(int count)
    {
        // Clear old candles
        foreach (var c in candles)
        {
            if (c != null) Destroy(c.gameObject);
        }

        candles.Clear();

        // Spawn new candles
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(initialX + (i * spacing), -11.217f, -0.26f); // fixed y and z
            Candle newCandle = Instantiate(candlePrefab, pos, Quaternion.identity, candleParent);
            candles.Add(newCandle);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
