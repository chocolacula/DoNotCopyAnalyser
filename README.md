This analyzer brings to the table a bit of functional programming by providing some statelessness.
It can protect you from stupid errors in some projects

It's useful in case of working with kinda global data like singleton. For example you have this:

```csharp
class SomeSingetonClass
{
    // singleton code...

    public readonly Data = new Data();
}
```

And you decided to cache `Data` in a private field inside another one class to stop use `SomeSingetonClass.Instance.Data.SubField1.SubField2.FieldName` in a code. In this case you would screw up if `Data` in `SomeSingetonClass` changed unexpectedly by deserialisation or by somebody else in another thread, etc.

To prevent this behaviour you can add `[DoNotCopy]` attribute to the field. 

```csharp
class SomeSingetonClass
{
    // singleton code...

    [DoNotCopy]
    public readonly Data = new Data();
}
```
You cannot copy the object like underlying pointer anymore, now you have one less state.
It will lead to a compilation error and you'll have to use your changable global data directly.
