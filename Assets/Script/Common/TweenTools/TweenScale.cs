using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TweenTools
{
    [RequireComponent(typeof(Transform))]
    public class TweenScale : TweenBase
    {
        [Header("演出結束就關閉物件")]
        public bool PlayEndThenHide = false;
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;

        private Transform trans;
        private Vector3 scale;

        //public override void OnEnable()
        //{
        //    if (trans != null)
        //    {
        //        scale = from;
        //        trans.localScale = scale;
        //    }
        //}

        public override void OnInit()
        {
            trans = transform;
            scale = from;

            if (base.playing == true)
            {
                trans.localScale = scale;
            }
        }

        public override void OnUpdate(float value)
        {
            if (base.playing == true)
            {
                scale = from + value * (to - from);
                trans.localScale = scale;
            }

            if (base.playing == false && PlayEndThenHide)
            {
                gameObject.SetActive(false);
            }

        }
    }
}