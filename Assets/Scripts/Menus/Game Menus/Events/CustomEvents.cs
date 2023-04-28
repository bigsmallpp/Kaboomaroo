using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventInt : UnityEvent<int>
{
    
}

[Serializable]
public class UnityEventFloat : UnityEvent<float>
{
    
}

[Serializable]
public class UnityEventString : UnityEvent<string>
{
    
}

[Serializable]
public class UnityEventDoubleInt : UnityEvent<DoubleInt>
{
    
}

[Serializable]
public class UnityEventBool : UnityEvent<bool>
{
    
}

public struct DoubleInt
{
    public DoubleInt(int first, int second)
    {
        _first = first;
        _second = second;
    }
    
    public int _first;
    public int _second;
}
