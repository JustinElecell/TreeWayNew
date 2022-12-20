using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TweenTools
{
    [RequireComponent(typeof(Transform))]
    public class TweenRotate : TweenBase
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;

        [Header("相對模式, 無論原本是什麼角度, 直接旋轉物件 eulerAngle 角度")]
        public bool relateRotateMode = false;
        public Vector3 eulerAngle = Vector3.zero;

        private Transform trans;
        private Vector3 scale;

        public override void OnInit()
        {
            trans = transform;

            //指定模式, 角度從 from 轉到 to
            if (!relateRotateMode)
            {
                scale = from;
                trans.rotation = Quaternion.Euler(scale);
            }
            //相對模式, 設定目前狀態的角度為 from , 增加 eulerAngle 角度設定為 to
            else
            {
                scale = from = trans.eulerAngles;
                to = from + eulerAngle;
            }
        }

        public override void OnUpdate(float value)
        {
            scale = from + value * (to - from);
            trans.rotation = Quaternion.Euler(scale);
        }
    }
}