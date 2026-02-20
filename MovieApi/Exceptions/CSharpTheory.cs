// This file is part of the project. Copyright (c) Company.

namespace MovieApi.Exceptions;

public class CSharpTheory
{
    // private string name;
    //
    // public string Name
    // {
    //     get => name;
    //     set { name = value; }
    // }

    public string Name { get; set; }

    private int[] array;

    private void CreateArray()
    {
        array = new[] { 1, 2, 3, 4 };
        var array2 = new[] { "string", new object() };
    }
}
