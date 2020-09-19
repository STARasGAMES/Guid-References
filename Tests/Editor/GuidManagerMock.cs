using System;
using UnityEngine;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidManagerMock : IGuidManager
    {
        /// <summary>
        /// Gets guid that was provided in last Add() method call.
        /// </summary>
        public Guid AddGuidArgument { get; private set; } = Guid.Empty;
        /// <summary>
        /// Gets gameObject that was provided in last Add() method call.
        /// </summary>
        public GameObject AddGameObjectArgument { get; private set; } = null;
        /// <summary>
        /// Gets guid that was provided in last Remove() method call.
        /// </summary>
        public Guid RemoveGuidArgument { get; private set; } = Guid.Empty;
        public bool AddResult { get; set; } = true;
        public bool RemoveResult { get; set; } = true;
        public GameObject ResolveGuidResult { get; set; } = null;

        private Action<GameObject> onAddCallback;
        private Action onRemoveCallback;
        
        public void InvokeAddCallback(GameObject gameObject)
        {
            onAddCallback?.Invoke(gameObject);
        }

        public void InvokeRemoveCallback()
        {
            onRemoveCallback?.Invoke();
        }
        
        public bool Add(Guid guid, GameObject gameObject)
        {
            AddGuidArgument = guid;
            AddGameObjectArgument = gameObject;
            return AddResult;
        }

        public bool Remove(Guid guid)
        {
            RemoveGuidArgument = guid;
            return RemoveResult;
        }

        public GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            this.onAddCallback = onAddCallback;
            this.onRemoveCallback = onRemoveCallback;
            return ResolveGuidResult;
        }

        public void ClearArguments()
        {
            AddGuidArgument = Guid.Empty;
            AddGameObjectArgument = null;
            RemoveGuidArgument = Guid.Empty;
        }
    }
}