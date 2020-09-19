using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace SaG.GuidReferences
{
    /// <summary>
    /// Provides reference to an GameObject by Guid.
    /// </summary>
    // note: Ideally this would be a struct, but we need the ISerializationCallbackReceiver which is not working with structs.
    [Serializable]
    public sealed class GuidReference : ISerializationCallbackReceiver
    {
        // cache the referenced Game Object if we find one for performance
        private GameObject cachedReference;

        // Indicates that cachedReference is set
        private bool isCacheSet;

        // Indicates that resolve request is sent. Doesn't mean that cachedReference is set.
        private bool isRequestSent;

        // store our GUID in a form that Unity can save
        [SerializeField] private byte[] serializedGuid;
        private Guid guid;

        // create concrete delegates to avoid boxing. 
        // When called 10,000 times, boxing would allocate ~1MB of GC Memory
        private Action<GameObject> addDelegate;
        private Action removeDelegate;
        
#if UNITY_EDITOR
        // decorate with some extra info in Editor so we can inform a user of what that GUID means
        [SerializeField] private string cachedName;
        [SerializeField] private SceneAsset cachedScene;
#endif
        
        /// <summary>Initializes a new instance of the <see cref="GuidReference"/> class.</summary>
        public GuidReference()
        {
            addDelegate = OnGuidAdded;
            removeDelegate = OnGuidRemoved;
        }

        /// <summary>Initializes a new instance of the <see cref="GuidReference"/> class.</summary>
        public GuidReference(Guid guid)
        {
            this.guid = guid;
            addDelegate = OnGuidAdded;
            removeDelegate = OnGuidRemoved;
        }

        // Set up events to let users register to cleanup their own cached references on destroy or to cache off values
        /// <summary>
        /// Occurs when reference is set. Use this event to get reference.
        /// </summary>
        public event Action<GameObject> Added = delegate(GameObject go) { };
        
        /// <summary>
        /// Occurs when reference is removed. In most cases this means that GameObject has been destroyed. 
        /// </summary>
        public event Action Removed = delegate() { };

        /// <summary>
        /// Returns target GameObject if it is loaded, otherwise, makes request to resolve reference and returns NULL. 
        /// Optimized accessor, and ideally the only code you ever call on this class.
        /// </summary>
        public GameObject gameObject
        {
            get
            {
                // return cached reference if it exists
                if (isCacheSet)
                    return cachedReference;

                GameObject reference = null;
                // if never asked for reference then ask for it
                if (!isRequestSent)
                {
                    reference = GuidManagerSingleton.ResolveGuid(guid, addDelegate, removeDelegate);
                    isRequestSent = true;
                }

                // if reference is null then explicitly return null 
                if (reference == null)
                    return null;

                // if cachedReference is not null then we can set isCacheSet to true.
                OnGuidAdded(reference);
                return reference;
            }
        }

        public void RequestResolve()
        {
            GuidManagerSingleton.ResolveGuid(guid, addDelegate, removeDelegate);
        }

        private void OnGuidAdded(GameObject go)
        {
            isCacheSet = true;
            cachedReference = go;
            Added?.Invoke(go);
        }

        private void OnGuidRemoved()
        {
            cachedReference = null;
            isCacheSet = false;
            isRequestSent = false;
            Removed?.Invoke();
        }

        // convert system guid to a format unity likes to work with
        public void OnBeforeSerialize()
        {
            serializedGuid = guid.ToByteArray();
        }

        // convert from byte array to system guid and reset state
        public void OnAfterDeserialize()
        {
            cachedReference = null;
            isCacheSet = false;
            if (serializedGuid == null || serializedGuid.Length != 16)
            {
                serializedGuid = new byte[16];
            }

            guid = new Guid(serializedGuid);
        }
    }
}