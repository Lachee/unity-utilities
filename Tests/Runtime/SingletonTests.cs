using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;

public class SingletonTests
{
    [UnityTearDown]
    public IEnumerator Teardown()
    {
        foreach (var go in GameObject.FindObjectsOfType<ExampleSingletonMonobehaviour>())
            GameObject.Destroy(go);

        yield return new WaitForEndOfFrame();
        ExampleSingletonMonobehaviour.instance = null;
    }

    [UnityTest]
    public IEnumerator FindInstance()
    {
        GameObject go = new GameObject();
        go.AddComponent<ExampleSingletonMonobehaviour>();
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(ExampleSingletonMonobehaviour.instance);
        Assert.IsTrue(ExampleSingletonMonobehaviour.referenced, "Reference will update with instance");
    }

    [UnityTest]
    public IEnumerator FindReference()
    {
        Assert.IsFalse(ExampleSingletonMonobehaviour.referenced, "Reference does not exist");
        Assert.IsFalse(ExampleSingletonMonobehaviour.referenced, "Referenced does not create a new instance");

        GameObject go = new GameObject();
        go.AddComponent<ExampleSingletonMonobehaviour>();
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(ExampleSingletonMonobehaviour.referenced, "Reference does not lookup");
    }

    [UnityTest]
    public IEnumerator FindAvailable()
    {
        Assert.IsFalse(ExampleSingletonMonobehaviour.available);
        Assert.IsFalse(ExampleSingletonMonobehaviour.available, "Available does not create a new instance");

        GameObject go = new GameObject();
        go.AddComponent<ExampleSingletonMonobehaviour>();
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(ExampleSingletonMonobehaviour.available, "Available does lookup");
    }

    [UnityTest]
    public IEnumerator InstanceCreates()
    {
        Assert.IsFalse(ExampleSingletonMonobehaviour.available);

        var instance = ExampleSingletonMonobehaviour.instance;
        Assert.IsNotNull(instance, "Instance will create a new gameobject");
        Assert.IsTrue(ExampleSingletonMonobehaviour.referenced, "Reference will update with instance");
        Assert.IsNotNull(GameObject.Find($"[ {ExampleSingletonMonobehaviour.type} INSTANCE ]"), "Game Object is created");
        yield return null;
    }

    [UnityTest]
    public IEnumerator AvailableDoesNotCreate()
    {
        Assert.IsFalse(ExampleSingletonMonobehaviour.available);

        var instance = ExampleSingletonMonobehaviour.available;
        Assert.IsNull(instance, "available will create not create a gameobject");
        Assert.IsFalse(ExampleSingletonMonobehaviour.referenced, "Reference will update with instance");
        Assert.That(GameObject.FindObjectsOfType<ExampleSingletonMonobehaviour>(), Is.Empty ,"No gameobjects were created");
        yield return null;
    }
}
