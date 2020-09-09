using System;
using UnityEngine;

namespace SaG.GuidReferences.Tests.Editor
{
    public class GuidManagerMock : IGuidManager
    {
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
        
        public bool Add(GuidComponent guidComponent)
        {
            return AddResult;
        }

        public bool Remove(Guid guid)
        {
            return RemoveResult;
        }

        public GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            this.onAddCallback = onAddCallback;
            this.onRemoveCallback = onRemoveCallback;
            return ResolveGuidResult;
        }
    }
}