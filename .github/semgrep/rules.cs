using System;
using UnityEngine;
class Example
{
    Example()
    {
        // ruleid: unity-random-in-constructor
        var roll = UnityEngine.Random.Range(0, 10);
        // ruleid: unity-random-in-constructor
        var value = UnityEngine.Random.value;
    }
    void Start()
    {
        // ok: unity-random-in-constructor
        var roll = UnityEngine.Random.Range(0, 10);
    }
    void Work()
    {
        try { Start(); }
        catch (Exception error)
        {
            // ruleid: csharp-preserve-exception-stack
            throw error;
        }
    }
    void Safe()
    {
        try { Start(); }
        catch (Exception error)
        {
            // ok: csharp-preserve-exception-stack
            throw;
        }
    }
}
class PublicConstructor
{
    public PublicConstructor()
    {
        // ruleid: unity-random-in-constructor
        var roll = Random.Range(0, 10);
    }
    public int RuntimeRoll()
    {
        // ok: unity-random-in-constructor
        return Random.Range(0, 10);
    }
}
