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
    public void Add()
    {
        Dictionary<string, float> weights = new Dictionary<string, float>();

        // PRepare the list and ensure everything is identical
        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 25f }
        };
        Assert.That(list.Count, Is.EqualTo(3), "total count is 3");
        Assert.That(list.TotalWeight, Is.EqualTo(100), "total weight is 100");

        weights = list.GetWeights().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.That(weights["A"], Is.EqualTo(50f), "item maintains weight");
        Assert.That(weights["B"], Is.EqualTo(25f), "item maintains weight");
        Assert.That(weights["C"], Is.EqualTo(25f), "item maintains weight");

        // Add D and make sure everything still matches
        list.Add("D", 25f);
        Assert.That(list.Count, Is.EqualTo(4));
        Assert.That(list.TotalWeight, Is.EqualTo(125), "total weight is 125");

        weights = list.GetWeights().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.That(weights["A"], Is.EqualTo(50f), "item maintains weight");
        Assert.That(weights["B"], Is.EqualTo(25f), "item maintains weight");
        Assert.That(weights["C"], Is.EqualTo(25f), "item maintains weight");
        Assert.That(weights["D"], Is.EqualTo(25f), "item maintains weight");

        // Add E and make sure everything still matches
        list.Add(new KeyValuePair<string, float>("D", 25f));
        Assert.That(list.Count, Is.EqualTo(5));
        Assert.That(list.TotalWeight, Is.EqualTo(150));

        // Add E and make sure everything still matches
        list.Add(("E", 25f));
        Assert.That(list.Count, Is.EqualTo(6));
        Assert.That(list.TotalWeight, Is.EqualTo(175));
    }

    [Test]
    public void Remove()
    {
        Dictionary<string, float> weights = new Dictionary<string, float>();

        // PRepare the list and ensure everything is identical
        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 25f }
        };
        Assert.That(list.Count, Is.EqualTo(3), "total count is 3");
        Assert.That(list.TotalWeight, Is.EqualTo(100), "total weight is 100");

        // Remove C
        Assert.That(list.Remove("C"), Is.True, "removed c");
        Assert.That(list.Count, Is.EqualTo(2), "total count is 2");
        Assert.That(list.TotalWeight, Is.EqualTo(75), "total weight is 75");

        // Remove non-existent value
        Assert.That(list.Remove("X"), Is.False, "didnt removed X");
        Assert.That(list.Count, Is.EqualTo(2), "total count is 2");
        Assert.That(list.TotalWeight, Is.EqualTo(75), "total weight is 75");
    }

    [Test]
    public void Weights()
    {
        // PRepare the list and ensure everything is identical
        float weight;
        bool tryGetWeight;
        bool trySetWeight;
        Dictionary<string, float> weights;

        RandomList<string> list = new RandomList<string>()
        {
            { "A", 50f },
            { "B", 25f },
            { "C", 25f }
        };
        Assert.That(list.TotalWeight, Is.EqualTo(100), "total weight is 100");

        trySetWeight = list.SetWeight("B", 50);
        Assert.That(trySetWeight, Is.True);
        Assert.That(list.GetWeight("B"), Is.EqualTo(50));
        Assert.That(list.TotalWeight, Is.EqualTo(125));

        tryGetWeight = list.TryGetWeight("D", out weight);
        Assert.That(tryGetWeight, Is.False);

        tryGetWeight = list.TryGetWeight("A", out weight);
        Assert.That(tryGetWeight, Is.True);
        Assert.That(weight, Is.EqualTo(50));

        weights = list.GetWeights().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.That(weights.Count, Is.EqualTo(3));
        Assert.That(weights.ContainsKey("A"));
        Assert.That(weights.ContainsKey("B"));
        Assert.That(weights.ContainsKey("C"));

        Assert.That(weights["A"], Is.EqualTo(50f));
        Assert.That(weights["B"], Is.EqualTo(50f));
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
