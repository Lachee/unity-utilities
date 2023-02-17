using System.Collections;
using System.Collections.Generic;
using Lachee.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class MathlTests
{
    [Test]
    public void Map()
    {
        Assert.That(Mathl.Map(0.5f, 0, 1, -128, 128), Is.EqualTo(0));
        Assert.That(Mathl.Map(0.75f, 0, 1, -128, 128), Is.EqualTo(64));
        Assert.That(Mathl.Map(0.25f, 0, 1, -128, 128), Is.EqualTo(-64));
        Assert.That(Mathl.Map(0, -100, 100, 0, 1), Is.EqualTo(0.5f));
        Assert.That(Mathl.Map(50, -100, 100, 0, 1), Is.EqualTo(0.75f));
        Assert.That(Mathl.Map(-50, -100, 100, 0, 1), Is.EqualTo(0.25f));
    }

    [Test]
    public void Mod()
    {
        Assert.That(Mathl.Mod(5, 10), Is.EqualTo(5), "Modulo doesn't effect small values");
        Assert.That(Mathl.Mod(15, 10), Is.EqualTo(5), "Modulo wraps positive");
        Assert.That(Mathl.Mod(-5, 10), Is.EqualTo(5), "Modulo wraps negative");
    }

    [Test]
    public void BitsSet()
    {
        Assert.That(Mathl.BitsSet(0b1), Is.EqualTo(1));
        Assert.That(Mathl.BitsSet(0b001100), Is.EqualTo(2));
        Assert.That(Mathl.BitsSet(int.MaxValue), Is.EqualTo((sizeof(int) * 8) - 1), "Max Value contains the number of bits equal to its size (minus the sign)");
    }

    [Test]
    public void Overlaps()
    {
        Rect overlap;
        Rect baseRect = new Rect(0, 0, 10, 10);

        Rect bottomRightRect = new Rect(5, 5, 10, 10);
        Assert.That(Mathl.Overlaps(baseRect, bottomRightRect, out overlap), Is.True, "Bottom Right returns true");
        Assert.That(overlap.size, Is.EqualTo(new Vector2(5, 5)));
        Assert.That(overlap.position, Is.EqualTo(new Vector2(5, 5)));

        Rect topLeftRect = new Rect(-5, -5, 10, 10);
        Assert.That(Mathl.Overlaps(baseRect, topLeftRect, out overlap), Is.True, "Top Left returns true");
        Assert.That(overlap.size, Is.EqualTo(new Vector2(5, 5)));
        Assert.That(overlap.position, Is.EqualTo(new Vector2(0, 0)));

        Rect totalInternalRect = new Rect(2.5f, 2.5f, 5f, 5f);
        Assert.That(Mathl.Overlaps(baseRect, totalInternalRect, out overlap), Is.True, "Total internal returns true");
        Assert.That(overlap, Is.EqualTo(totalInternalRect));

        Rect missed = new Rect(20f, 20f, 5f, 5f);
        Assert.That(Mathl.Overlaps(baseRect, missed, out overlap), Is.False, "External rects return false");
        Assert.That(overlap, Is.EqualTo(missed), "Missed rectangles return the other result");
    }

    [Test]
    public void Extrudes()
    {
        var comparer = new Vector3EqualityComparer(10e-6f);
        Assert.That(Mathl.Extrude(new Vector3(0, 1, 0), Vector3.zero, 10f), Is.EqualTo(new Vector3(0, 11, 0)).Using(comparer), "Extrude moves the vector away from the center (up)");
        Assert.That(Mathl.Extrude(new Vector3(0, -1, 0), Vector3.zero, 10f), Is.EqualTo(new Vector3(0, -11, 0)).Using(comparer), "Extrude moves the vector away from the center (down)");
    }
}
