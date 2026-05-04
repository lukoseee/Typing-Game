using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//manages the candles in the scene
public class CandleController : MonoBehaviour
{   
    private List<Candle> candles = new List<Candle>(); //all candles on the table (corresponds to words in level)
    [SerializeField] private int spacing = 2; //spacing between candles on table
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

        //light animation
        candles[index].Light();
        Debug.Log($"Lit candle {index} of {candles.Count}");

    }

    public void ResetCandles()
    {   
        
        for (int i = 0; i < candles.Count; i++)
        {   
            //unlit
            candles[i].ResetCandle();
        }
    }

    //spawn candles at interval on table
    public void SpawnCandles(int count)
    {   
        //clear existing candles
        foreach (var c in candles)
        {
            if (c != null) Destroy(c.gameObject);
        }

        //clear list
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
            //blow out animation
            candles[i].BlowOut();

            //staggered effect blows one by one from right to left
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
