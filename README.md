# DoNotCopy Analyzer

A Roslyn analyzer that brings a bit of functional programming to your codebase by reducing the amount of **state**. It protects you from silly mistakes in rare but painful cases.

It's useful when working with global-ish data, such as a singleton. Say you have this:

```csharp
class SomeSingletonClass
{
    // singleton code...
    public readonly Data Data = new Data();
}
```

At some point you decide to cache `Data` in a private field of another class, so you don't have to walk the whole chain every time:

```csharp
LoadPieceOfData(SomeSingletonClass.Instance.Data.FieldName);
```

This is especially tempting when there are several layers of nested objects above `Data`.

The problem is that you don't expect `Data` to be replaced — by deserialization, or by someone else on another thread. If that happens, you end up with two different `Data` instances: one cached, one in the singleton.

To prevent this, add the `[DoNotCopy]` attribute to the field:

```csharp
class SomeSingletonClass
{
    // singleton code...
    [DoNotCopy]
    public readonly Data Data = new Data();
}
```

The object can no longer be copied, so that's one less piece of state. Any attempt to copy it becomes a compilation error, forcing you to use your mutable global data directly. Less convenient, admittedly, but it keeps you from splitting the state. Use it wisely.
