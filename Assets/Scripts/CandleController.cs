using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandleController : MonoBehaviour
{   

    private List<Candle> candles = new List<Candle>();


    public int spacing = 2;
    public float initialX = -8.191f;
    public Candle candlePrefab;
    public Transform candleParent;
    public GameController gameController;

    public void lightNextCandle(int index)
    {   
        if(index >= candles.Count)
                return;

        candles[index].Light();
        Debug.Log($"Lit candle {index} of {candles.Count}");

    }

    public void ResetCandles()
    {   
        

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


    public Vector3 GetCandlePosition(int index)
    {
        if (index < 0 || index >= candles.Count)
            return Vector3.zero;

        return candles[index].transform.position;
    }

    public int CandleCount()
    {
        return candles.Count;
    }

    public IEnumerator BlowOutAll()
    {   
        // Start from the last lit candle
        for (int i = gameController.getCurrentCandleIndex() - 1; i >= 0; i--)
        {
            candles[i].BlowOut();

            // small delay between each candle
            yield return new WaitForSeconds(0.15f);
        }

    }
    
}
