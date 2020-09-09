# Guid References
Package for Unity that provides functionality to reference GameObjects based on Guid (Global Unique IDentifier).

Supports UPM (Unity Package Manager).

Inspired by https://github.com/Unity-Technologies/guid-based-reference. 

## Installation
Install via git url by adding this entry in your **manifest.json**

`"com.sag.guid-references": "https://github.com/STARasGAMES/Guid-References.git#upm"`

## Explanation
Unity provides easy to use and convenient solution to reference components through inspector. But it has limitation that this components have to be in the same scene.

Having a way to identify game object by some unique identifier (GUID) opens a lot of opportunities:
- cross-scene references
- reference game object that is not loaded/created yet
- create save system based on GUIDs

## Basic usage
1. Add `GuidComponent` to you game object. In the inspector you should see this component with auto-generated Guid. The system guarantees this Guid to be unique.

2. Then in your script create `GuidReference` serializable field:
```c#
public GuidReference guidReference;
```

3. Drag and drop game object with `GuidComponent` into the `GuidReference` field.

4. Use `GuidReference` in your script.
```c#
void Update()
{
  if (guidReference.gameObject != null) // this check is needed in case referenced game object is not loaded yet.
    guidReference.gameObject.transform.Rotate(0, 10, 0);
}
```


## Advanced usage
### Make use of events
Every frame check that reference is not NULL is a waste of resources. Alternatively, we can use events `Added` and `Removed` to populate and clear reference:
```c#
public GuidReference guidReference;
private GameObject cachedReferencedGameObject;

void Awake()
{
  guidReference.Added += go => cachedReferencedGameObject = go;
  guidReference.Removed += () => cachedReferencedGameObject = null;
}
```


### Manually resolve reference
Also, you can resolve references by yourself in case you don't want to use `GuidReference`:
```c#
private System.Guid customGuid; // you need to provide your own Guid to resolve reference.

void Initialize()
{
  var referencedGameObject = GuidManagerSingleton.ResolveGuid(customGuid);
}
```
There is version of `GuidManagerSingleton.ResolveGuid()` with `onAdded` and `onRemoved` callbacks.


### GuidManager class
`GuidManagerSingleton` is just an API-interface over `IGuidManager` interface. The default implementation is `GuidManager` class. `GuidManagerSingleton` used by `GuidComponent` and `GuidReference` classes.  

If you want to use DI you need to explicitly set guid manager instance, but be careful:
```c#
IGuidManager guidManager = new GuidManager();
...
GuidManagerSingleton.SetInstance(guidManager);
```
