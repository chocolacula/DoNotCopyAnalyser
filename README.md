This analyzer can protect you from stupid errors in some projects

It's useful with working with a kinda global data like singleton. For example you have this:

```csharp
class SomeSingetonClass
{
    // singleton code...

    public readonly Data = new Data();
}
```

And you decided to cache `Data` in a private field inside another class to stop use `SomeSingetonClass.Instance.Data.FieldName` in code. In this case you would screw up if `Data` in `SomeSingetonClass` changed by deserialisation from disk or network or by something else.

To prevent this case you can add `[DoNotCopy]` attribute to the field. 

```csharp
class SomeSingetonClass
{
    // singleton code...

    [DoNotCopy]
    public readonly Data = new Data();
}
```

It will go to a compilation error and you'll have to use your changable global data directly. 
