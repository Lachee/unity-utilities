using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lachee.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class RanomListTests
{
    const int TALLY_ITERATIONS = 100_000;
    const float TALLY_TOLERANCE = 0.01f;

    #region Unit Tests
    [Test]
    public void TotalWeight()
    {
        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 25f }
        };
        Assert.That(list.TotalWeight, Is.EqualTo(100), "total weight is 100");

        var weights = list.GetWeights().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.That(weights["A"], Is.EqualTo(50f));
        Assert.That(weights["B"], Is.EqualTo(25f));
        Assert.That(weights["C"], Is.EqualTo(25f));
    }

    [Test]
    public void Clear()
    {
        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 25f }
        };

        list.Clear();
        Assert.That(list.TotalWeight, Is.EqualTo(0), "total weight is 0");

        var weights = list.GetWeights().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.That(weights.Count, Is.EqualTo(0));
    }

    [Test]
    public void Randomise()
    {
        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 8f },
            { "D", 8f },
            { "E", 8f },
        };

        // Iterate 1000 times and see what the tallies are
        Dictionary<string, int> tally = new Dictionary<string, int>() {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 },
            { "D", 0 },
            { "E", 0 },
        };

        string result;
        bool foundResult;
        for(int i = 0; i < TALLY_ITERATIONS; i++)
        {
            foundResult = list.Randomise(out result);
            Assert.That(foundResult, Is.True, "always finds a result");
            Assert.That(tally.ContainsKey(result), "result is from the list");
            tally[result]++;
        }

        Assert.That((float)tally["A"] / TALLY_ITERATIONS, Is.EqualTo(0.5f).Within(TALLY_TOLERANCE), "A is approximately 50%");
        Assert.That((float)tally["B"] / TALLY_ITERATIONS, Is.EqualTo(0.25f).Within(TALLY_TOLERANCE), "B is approximately 25%");
        Assert.That((float)tally["C"] / TALLY_ITERATIONS, Is.EqualTo(0.08f).Within(TALLY_TOLERANCE), "C is approximately 8%");
        Assert.That((float)tally["D"] / TALLY_ITERATIONS, Is.EqualTo(0.08f).Within(TALLY_TOLERANCE), "D is approximately 8%");
        Assert.That((float)tally["E"] / TALLY_ITERATIONS, Is.EqualTo(0.08f).Within(TALLY_TOLERANCE), "E is approximately 8%");
    }
    #endregion


}
