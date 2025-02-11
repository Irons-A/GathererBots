using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [field: SerializeField] public List<Bot> Bots { get; private set; }
}
