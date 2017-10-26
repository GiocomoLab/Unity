using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NameCheck : MonoBehaviour
{

    public string Recurse(string fbase)
    {
        if (File.Exists(fbase + ".txt"))
        {
            fbase = fbase + "_copy";
            fbase = Recurse(fbase);
        }
        return fbase;
    }
}
