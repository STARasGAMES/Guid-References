using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidComponentTests
    {
        // Tests - make a new GUID
        // duplicate it
        // make it a prefab
        // delete it
        // reference it
        // dereference it

        const string PrefabPath = "Assets/TemporaryTestGuid.prefab";

        GuidComponent guidBase;
        GameObject prefab;
        GuidComponent guidPrefab;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            GuidManagerSingleton.SetInstance(new GuidManager());
            guidBase = CreateNewGuid();
            prefab = PrefabUtility.CreatePrefab(PrefabPath, guidBase.gameObject);

            guidPrefab = prefab.GetComponent<GuidComponent>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(PrefabPath);
        }
        
        public static GuidComponent CreateNewGuid()
        {
            GameObject newGO = new GameObject("GuidTestGO");
            return newGO.AddComponent<GuidComponent>();
        }
        
        [Test]
        public void GetGuid_ReturnsUniqueGuid_WhenNewGameObjectCreated()
        {
            GuidComponent guid1 = guidBase;
            GuidComponent guid2 = CreateNewGuid();

            Assert.AreNotEqual(guid1.GetGuid(), guid2.GetGuid());
        }

        [Test]
        public void GetGuid_ReturnsUniqueGuid_WhenInstantiatedFromGameObject()
        {
            LogAssert.Expect(LogType.Warning,
                "Guid Collision Detected while creating GuidTestGO(Clone).\nAssigning new Guid.");

            GuidComponent clone = Object.Instantiate<GuidComponent>(guidBase);

            Assert.AreNotEqual(guidBase.GetGuid(), clone.GetGuid());
        }

        [Test]
        public void GetGuid_ReturnsEmptyGuid_WhenItIsPrefab()
        {
            Assert.AreNotEqual(guidBase.GetGuid(), guidPrefab.GetGuid());
            Assert.AreEqual(guidPrefab.GetGuid(), System.Guid.Empty);
        }

        [Test]
        public void GetGuid_ReturnsUniqueGuid_WhenInstantiatedFromPrefab()
        {
            GuidComponent instance = Object.Instantiate<GuidComponent>(guidPrefab);
            Assert.AreNotEqual(guidBase.GetGuid(), instance.GetGuid());
            Assert.AreNotEqual(instance.GetGuid(), guidPrefab.GetGuid());
        }

        [Test]
        public void GuidComponent_RaisesRemovedCallback_WhenDestroyed()
        {
            var guidComponent = CreateNewGuid();
            int removeCallbackCount = 0;
            GuidManagerSingleton.ResolveGuid(guidComponent.GetGuid(), () => removeCallbackCount++);
            var guid = guidComponent.GetGuid();
            Object.DestroyImmediate(guidComponent.gameObject);
            Assert.AreEqual(1, removeCallbackCount);
        }
    }
}