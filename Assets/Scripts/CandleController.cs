using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandleController : MonoBehaviour
{   
    private List<Candle> candles = new List<Candle>();
    [SerializeField] private int spacing = 2;
    [SerializeField] private float initialX = -8.191f;
    [SerializeField] private Candle candlePrefab;
    [SerializeField] private Transform candleParent;
    [SerializeField] private GameController gameController;
    [SerializeField] private float height = -11.217f;
    [SerializeField] private float zPos = 80.7380f;

    private Coroutine blowOutRoutine;

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
        foreach (var c in candles)
        {
            if (c != null) Destroy(c.gameObject);
        }

        candles.Clear();

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(initialX + (i * spacing), height, zPos);
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
        
        for (int i = gameController.getCurrentCandleIndex() - 1; i >= 0; i--)
        {
            candles[i].BlowOut();

            yield return new WaitForSeconds(0.2f);
        }

    }

    public void StartBlowOutAll()
    {
        if (blowOutRoutine != null) StopCoroutine(blowOutRoutine);
        blowOutRoutine = StartCoroutine(BlowOutAll());
    }

    public void StopBlowOut()
    {
        if (blowOutRoutine != null)
        {
            StopCoroutine(blowOutRoutine);
            blowOutRoutine = null;
        }
    }

    public int GetSpacing()
    {
        return spacing;
    }
    
}
