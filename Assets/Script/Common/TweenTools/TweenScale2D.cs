using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TweenTools
{
    [RequireComponent(typeof(RectTransform))]
    public class TweenScale2D : TweenBase
    {
        [Header("演出結束就關閉物件")]
        public bool PlayEndThenHide = false;
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;

        private RectTransform trans;
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
            trans = GetComponent<RectTransform>();
            scale = from;

            if (base.playing == true)
            {
                trans.localScale = scale;
            }
        }

        public override void OnUpdate(float value)
        {
            //Debug.LogError("TweenShow");
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