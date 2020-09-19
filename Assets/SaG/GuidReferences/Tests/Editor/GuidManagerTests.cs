using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidManagerTests
    {
        private GuidManager guidManager;
        private GuidManagerMock guidManagerMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            guidManager = new GuidManager();
            guidManagerMock = new GuidManagerMock();
            // set mock guid manager as singleton so guid components will not conflict it test cases.
            GuidManagerSingleton.SetInstance(guidManagerMock);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            GuidManagerSingleton.SetInstance(new GuidManager());
        }

        [Test]
        public void Add_ThrowsArgumentNullException_WhenProvideNullGameObject()
        {
            Assert.Throws<ArgumentNullException>(() => guidManager.Add(Guid.NewGuid(), null));
        }
        
        [Test]
        public void Add_ThrowsArgumentNullException_WhenProvideEmptyGuid()
        {
            Assert.Throws<ArgumentNullException>(() => guidManager.Add(Guid.Empty, new GameObject("Add_ThrowsArgumentNullException_WhenProvideEmptyGuid")));
        }

        [Test]
        public void Add_ThrowsArgumentNullException_WhenProvideDestroyedGameObject()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            Object.DestroyImmediate(gameObject);
            Assert.Throws<ArgumentNullException>(() => guidManager.Add(guid, gameObject));
        }

        [Test]
        public void Add_ReturnsTrue_WhenProvideValidGuidComponent()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            Assert.IsTrue(guidManager.Add(guid, gameObject));
        }

        [Test]
        public void Add_ReturnsTrue_WhenProvideSameGuidComponentTwice()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            Assert.IsTrue(guidManager.Add(guid, gameObject));
            Assert.IsTrue(guidManager.Add(guid, gameObject));
        }

        [Test]
        public void Add_ReturnsFalse_WhenProvideGuidComponentsWithSameGuid()
        {
            var guid = Guid.NewGuid();
            var gameObject1 = new GameObject("GuidManagerTests GO 1");
            var gameObject2 = new GameObject("GuidManagerTests GO 2");
            Assert.IsTrue(guidManager.Add(guid, gameObject1));
            Assert.IsFalse(guidManager.Add(guid, gameObject2));
        }

        [Test]
        public void Add_RaisesAddedCallback()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            int addedCallbackRaiseCount = 0;
            GameObject addedCallbackResolveObject = null;
            guidManager.ResolveGuid(guid, o =>
                {
                    addedCallbackResolveObject = o;
                    addedCallbackRaiseCount++;
                },
                null);
            guidManager.Add(guid, gameObject);
            Assert.AreEqual(1, addedCallbackRaiseCount);
            Assert.AreEqual(gameObject, addedCallbackResolveObject);
        }
        
        [Test]
        public void Remove_ReturnsTrue_WhenProvideValidGuid()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            Assert.IsTrue(guidManager.Remove(guid));
        }

        [Test]
        public void Remove_ReturnsFalse_WhenProvideSameGuidTwice()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            guidManager.Remove(guid);
            Assert.IsFalse(guidManager.Remove(guid));
        }

        [Test]
        public void Remove_ReturnsFalse_WhenProvideEmptyGuid()
        {
            Assert.IsFalse(guidManager.Remove(Guid.Empty));
        }

        [Test]
        public void Remove_ReturnsFalse_WhenProvideInvalidGuid()
        {
            Assert.IsFalse(guidManager.Remove(Guid.NewGuid()));
        }
        
        [Test]
        public void Remove_RaisesRemovedCallback()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            int removedCallbackRaiseCount = 0;
            guidManager.ResolveGuid(guid, null,
                () =>
                {
                    removedCallbackRaiseCount++;
                });
            guidManager.Remove(guid);
            Assert.AreEqual(1, removedCallbackRaiseCount);
        }

        [Test]
        public void ResolveGuid_Returns_WhenProvideValidGuid()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            var resolveGuid = guidManager.ResolveGuid(guid, null, null);
            Assert.AreEqual(gameObject, resolveGuid);
        }

        [Test]
        public void ResolveGuid_ReturnsNull_WhenProvideInvalidGuid()
        {
            Assert.IsNull(guidManager.ResolveGuid(Guid.NewGuid(), null, null));
        }

        [Test]
        public void ResolveGuid_ReturnsNull_WhenProvideRemovedGuid()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            guidManager.Remove(guid);
            Assert.IsNull(guidManager.ResolveGuid(guid, null, null));
        }
        
        [Test]
        public void ResolveGuid_ReturnsNull_WhenGameObjectDestroyed()
        {
            var guid = Guid.NewGuid();
            var gameObject = new GameObject("GuidManagerTests GO");
            guidManager.Add(guid, gameObject);
            Object.DestroyImmediate(gameObject);
            Assert.IsTrue(guidManager.ResolveGuid(guid, null, null) == null); // unity's object check for null
        }
    }
}