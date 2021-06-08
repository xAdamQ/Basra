using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class asyncTst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public async void AwaitAndDebug()
    {
        await UniTask.Delay(1000);
        throw new Exception("an custom exc should appear");
        Debug.Log("done");
    }
}