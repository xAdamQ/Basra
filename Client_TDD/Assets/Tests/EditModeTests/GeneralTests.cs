using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GeneralTests
{
    [Test]
    public void GeneralTestsSimplePasses()
    {
        Debug.Log(TimeSpan.MinValue);
        Debug.Log(TimeSpan.MaxValue);
        Debug.Log(TimeSpan.MaxValue + TimeSpan.MinValue);
        Debug.Log(TimeSpan.FromSeconds(7) - TimeSpan.FromSeconds(10));
    }
}