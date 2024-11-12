using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum thachsanh { BODY, LEGS }

public class AnyStateAnimation 
{
    public thachsanh AnimationTS { get; private set; }
    public string Name { get; set; }

    public bool Active { get; set; }

    public AnyStateAnimation(thachsanh ts, string name)
    {
        this.AnimationTS = ts;
        this.Name = name;
    }
}
