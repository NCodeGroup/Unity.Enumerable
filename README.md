# Overview
> PM> Install-Package Unity.Enumerable

This library provides an extension to the [Unity Container] from Microsoft to add support for resolving multiple objects by using `IEnumerable<T>`.

## Before
Normally with Unity Container to resolve multiple objects you would use the builtin method `ResolveAll` like so:

```csharp
IUnityContainer container = CreateContainer();

IEnumerable<IMyObject> myObjects = container.ResolveAll<IMyObject>();
```

## After
Instead, this library provides an extension to Unity Container that allows you to resolve multiple objects using `Resolve<IEnumerable<T>>` like so:

```csharp
IUnityContainer container = CreateContainer();

container.AddNewExtension<EnumerableExtension>();

IEnumerable<IMyObject> myObjects = container.Resolve<IEnumerable<IMyObject>>();
```

All that is required is to add `EnumerableExtension` to your Unity Container as shown above.

## Lazy Support
In additional to resolving `IEnumerable<T>`, this extension also supports resolving `ILazy<IEnumerable<T>>` like so:

```csharp
IUnityContainer container = CreateContainer();

container.AddNewExtension<EnumerableExtension>();

Lazy<IEnumerable<IMyObject>> myLazyObjects = container.Resolve<Lazy<IEnumerable<IMyObject>>>();

IEnumerable<IMyObject> myObjects = myLazyObjects.Value;
```

## Unnamed Registrations
This extension does change the default behvior of the Unity Container in regards to unnamed (i.e. default with no name) registrations. Previously Unity would not return the default unnamed registration when using `ResolveAll`. But this extension changes that behavior and those default unnamed registrations will be included with `ResolveAll` and `Resolve<IEnumerable<T>>`.

## Release Notes
* v1.0.0 - Initial release

## Feedback
Please provide any feedback, comments, or issues to this GitHub project [here][issues].

[Unity Container]: https://github.com/unitycontainer/unity
[issues]: https://github.com/NCodeGroup/Unity.Enumerable/issues
