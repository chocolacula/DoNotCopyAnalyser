using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
class DoNotCopyAttribute : Attribute
{}
