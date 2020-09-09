using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
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
        public void Add_ThrowsArgumentNullException_WhenProvideNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => guidManager.Add(null));
        }

        [Test]
        public void Add_ThrowsMissingReferenceException_WhenProvideDestroyedGameObject()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            Object.DestroyImmediate(guidComponent.gameObject);
            Assert.Throws<MissingReferenceException>(() => guidManager.Add(guidComponent));
        }

        [Test]
        public void Add_ReturnsTrue_WhenProvideValidGuidComponent()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            Assert.IsTrue(guidManager.Add(guidComponent));
        }

        [Test]
        public void Add_ReturnsTrue_WhenProvideSameGuidComponentTwice()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            Assert.IsTrue(guidManager.Add(guidComponent));
            Assert.IsTrue(guidManager.Add(guidComponent));
        }

        [Test]
        public void Add_ReturnsFalse_WhenProvideGuidComponentsWithSameGuid()
        {
            var guidComponent1 = GuidComponentTests.CreateNewGuid();
            var guidComponent2 = GuidComponentTests.CreateNewGuid();
            guidComponent2.SetGuid(guidComponent1.GetGuid());
            Assert.IsTrue(guidManager.Add(guidComponent1));
            Assert.IsFalse(guidManager.Add(guidComponent2));
        }
        

        [Test]
        public void Add_RaisesAddedCallback()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            guidManager.Remove(guidComponent.GetGuid());
            int addedCallbackRaiseCount = 0;
            GameObject addedCallbackResolveObject = null;
            guidManager.ResolveGuid(guidComponent.GetGuid(), o =>
                {
                    addedCallbackResolveObject = o;
                    addedCallbackRaiseCount++;
                },
                null);
            guidManager.Add(guidComponent);
            Assert.AreEqual(1, addedCallbackRaiseCount);
            Assert.AreEqual(guidComponent.gameObject, addedCallbackResolveObject);
        }
        
        [Test]
        public void Remove_ReturnsTrue_WhenProvideValidGuid()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            Assert.IsTrue(guidManager.Remove(guidComponent.GetGuid()));
        }

        [Test]
        public void Remove_ReturnsFalse_WhenProvideSameGuidTwice()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            guidManager.Remove(guidComponent.GetGuid());
            Assert.IsFalse(guidManager.Remove(guidComponent.GetGuid()));
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
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            int removedCallbackRaiseCount = 0;
            guidManager.ResolveGuid(guidComponent.GetGuid(), null,
                () =>
                {
                    removedCallbackRaiseCount++;
                });
            guidManager.Remove(guidComponent.GetGuid());
            Assert.AreEqual(1, removedCallbackRaiseCount);
        }

        [Test]
        public void ResolveGuid_Returns_WhenProvideValidGuid()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            var resolveGuid = guidManager.ResolveGuid(guidComponent.GetGuid(), null, null);
            Assert.AreEqual(guidComponent.gameObject, resolveGuid);
        }

        [Test]
        public void ResolveGuid_ReturnsNull_WhenProvideInvalidGuid()
        {
            Assert.IsNull(guidManager.ResolveGuid(Guid.NewGuid(), null, null));
        }

        [Test]
        public void ResolveGuid_ReturnsNull_WhenProvideRemovedGuid()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            guidManager.Add(guidComponent);
            guidManager.Remove(guidComponent.GetGuid());
            Assert.IsNull(guidManager.ResolveGuid(guidComponent.GetGuid(), null, null));
        }
    }
}