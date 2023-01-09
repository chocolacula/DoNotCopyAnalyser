# DoNotCopy Analyzer

It brings to the table a bit of functional programming by reducing amount of **state**.  
It can protect you from stupid errors in rare but painful cases.

It's useful in case of working with kinda global data like singleton. For example you have this:

```csharp
class SomeSingetonClass
{
    // singleton code...

    public readonly Data = new Data();
}
```

And you decided to cache `Data` in a private field inside another class  
to stop use `Data.FieldName` through whole chain like:

```csharp
LoadPieceOfData(SomeSingetonClass.Instance.Data.FieldName);
```
Especially if you have a few layers of nested objects above `Data`.

You definitely don't expect `Data` changed by deserialisation or by somebody in another thread.  
And if it happend, you will have two different `Data` - one cached, one in the singleton.  

To prevent that you can add `[DoNotCopy]` attribute to the field. 

```csharp
class SomeSingetonClass
{
    // singleton code...

    [DoNotCopy]
    public readonly Data = new Data();
}
```
You cannot copy the object anymore, now you have one less state.  
It will lead to a compilation error and you'll have to use your changable global data directly.  
It's not handy unfortunately but keeps you away from splitting the state. Use it wisely.
