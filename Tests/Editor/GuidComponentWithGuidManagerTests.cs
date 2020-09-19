using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidComponentWithGuidManagerTests
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

        [SetUp]
        public void Setup()
        {
            guidManagerMock.ClearArguments();
        }

        [Test]
        public void GuidComponent_AddsToGuidManager_WhenInstantiated()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            Assert.AreEqual(guidComponent.GetGuid(), guidManagerMock.AddGuidArgument);
            Assert.AreEqual(guidComponent.gameObject, guidManagerMock.AddGameObjectArgument);
        }
        
        [Test]
        public void GuidComponent_RemovesToGuidManager_WhenDestroyed()
        {
            var guidComponent = GuidComponentTests.CreateNewGuid();
            var guid = guidComponent.GetGuid();
            Object.DestroyImmediate(guidComponent.gameObject);
            Assert.AreEqual(guid, guidManagerMock.RemoveGuidArgument);
        }
        
        // todo test when IGuidManager.Add() returns false
    }
}