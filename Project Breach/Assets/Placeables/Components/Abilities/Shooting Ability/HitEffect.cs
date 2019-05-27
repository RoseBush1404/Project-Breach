using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    public class HitEffect : MonoBehaviour
    {
        void Start()
        {
            Destroy(transform.parent.gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        }
    }
}