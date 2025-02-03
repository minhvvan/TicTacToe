using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private B b;
    
    void Start()
    {
        b = new B(CallFromB);
    }

    public void CallFromB() 
    {
        Debug.Log("CallFromB");
    }
}

class Animal
{
    public virtual void MakeSound()
    {
        Debug.Log("Some animal sound...");
    }
}
class Dog : Animal
{
    public override void MakeSound()
    {
        Debug.Log("Woof! Woof!");
    }
}

class B 
{
    public delegate void Delegate();
    public Delegate callback;
    
    public B(Delegate callback) 
    {
        this.callback = callback;
    }
    
    private void callA()
    {
        callback();
    }
}