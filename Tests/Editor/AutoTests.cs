using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using Lachee.Attributes;

public class AutoTests
{
    private EditorWindow _lockableInspectorWindow;

    [SetUp]
    public void OnSetup()
    {
        _lockableInspectorWindow = Utils.CreateInspector();
    }
    
    [TearDown]
    public void OnTearDown()
    {
        if (_lockableInspectorWindow)
            _lockableInspectorWindow.Close();
    }
    

    [UnityTest]
    public IEnumerator CanFindOnOwnComponent()
    {
        GameObject gameObject;
        BoxCollider boxCollider;
        AutoBasicMonoBehaviour behaviour;

        gameObject = new GameObject();
        boxCollider = gameObject.AddComponent<BoxCollider>();
        behaviour = gameObject.AddComponent<AutoBasicMonoBehaviour>();
        yield return InspectObject(gameObject);

        Assert.That(behaviour.boxCollider, Is.EqualTo(boxCollider));
    }


    [UnityTest]
    public IEnumerator ComplainsEmpty()
    {
        GameObject gameObject;
        BoxCollider boxCollider;
        AutoBasicMonoBehaviour behaviour;

        gameObject = new GameObject();
        behaviour = gameObject.AddComponent<AutoBasicMonoBehaviour>();
        yield return InspectObject(gameObject);

        //Assert.That(behaviour.boxCollider, Is.EqualTo(boxCollider));
    }

    protected IEnumerator InspectObject(Object obj)
    {
        Utils.LockInspector(_lockableInspectorWindow, new List<Object>() { obj });
        yield return null;
        
    }

    internal class AutoBasicMonoBehaviour : MonoBehaviour
    {
        [Auto(SearchFlag = AutoSearchFlag.GameObject)]
        public BoxCollider boxCollider;
    }
}

