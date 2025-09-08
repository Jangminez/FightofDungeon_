using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamgeable
{
    void Hit(float damage, bool isCritical);
}
