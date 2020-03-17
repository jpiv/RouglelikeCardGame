using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;


public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestTest()
        {
            Assert.That(GameClass.GetTestData() == 5);
            // Use the Assert class to test conditions
        }
        [Test]
        public void CatchingErrors()
        {

        GameObject gameObject = new GameObject("test");

        Assert.Throws<MissingComponentException>(
        () => gameObject.GetComponent<Rigidbody>().velocity = Vector3.one);

}

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }

