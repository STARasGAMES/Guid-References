using NUnit.Framework;
using UnityEngine;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidReferenceTests
    {
        private GuidManagerMock guidManagerMock;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            guidManagerMock = new GuidManagerMock();
            GuidManagerSingleton.SetInstance(guidManagerMock);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            GuidManagerSingleton.SetInstance(new GuidManager());
        }
        
        [Test]
        public void GuidReference_ReturnsNull_WhenGuidIsNotSet()
        {
            GuidReference reference = new GuidReference();
            Assert.IsNull(reference.gameObject);
        }
        
        [Test]
        public void GuidReference_ReturnsNull_WhenTargetGameObjectDestroyed()
        {
            GuidComponent newGuid = GuidComponentTests.CreateNewGuid();
            GuidReference reference = new GuidReference(newGuid);
            // todo
            Object.DestroyImmediate(newGuid);

            Assert.IsNull(reference.gameObject);
        }

        [Test]
        public void GuidReference_ReturnsGameObject_WhenValidReference()
        {
            GuidComponent newGuid = GuidComponentTests.CreateNewGuid();
            GuidReference reference = new GuidReference(newGuid);
            guidManagerMock.ResolveGuidResult = newGuid.gameObject;
            Assert.AreEqual(newGuid.gameObject, reference.gameObject);
        }
        
        [Test]
        public void AddedEvent_Raises_WhenGuidAdded()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            GuidReference reference = new GuidReference(guidComponent);
            int addedEventRaiseCount = 0;
            GameObject addedEventResult = null;
            reference.Added += gameObject =>
            {
                addedEventRaiseCount++;
                addedEventResult = gameObject;
            };
            reference.RequestResolve();
            
            guidManagerMock.InvokeAddCallback(guidComponent.gameObject);
            
            // main test
            Assert.AreEqual(1, addedEventRaiseCount);
            Assert.AreEqual(guidComponent.gameObject, addedEventResult);
        }
        
        [Test]
        public void RemovedEvent_Raises_WhenGuidRemoved()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            GuidReference reference = new GuidReference(guidComponent);
            reference.RequestResolve();
            int eventRaiseCount = 0;
            reference.Removed += () => eventRaiseCount++;
            guidManagerMock.InvokeRemoveCallback();
            // main test
            Assert.AreEqual(1, eventRaiseCount);
        }
    }
}