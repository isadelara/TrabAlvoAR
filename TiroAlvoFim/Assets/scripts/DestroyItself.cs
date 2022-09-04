using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItself : MonoBehaviour
{
    public GameObject obj;
    public void destroy() { Destroy(obj); }
    public void destroy( float time ) { Invoke(nameof(destroy), time); }
}
