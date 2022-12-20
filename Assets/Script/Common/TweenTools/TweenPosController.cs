using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TweenTools
{
    [RequireComponent(typeof(TweenPosition))]
    public class TweenPosController : MonoBehaviour
    {
        private TweenPosition tp;
        private Vector3 normalModeV3;
        private Vector3 comebackModeV3;
        private bool isNormalMode = true;

        // Start is called before the first frame update
        void Awake()
        {
            tp = gameObject.GetComponent<TweenPosition>();
            normalModeV3 = tp.from;
            comebackModeV3 = tp.to;
            tp.enabled = false;
        }

        // Update is called once per frame
        public void TranMode()
        {
            tp.enabled = false;
            if (isNormalMode)
            {
                tp.from = normalModeV3;
                tp.to = comebackModeV3;
                isNormalMode = false;
            }
            else
            {
                tp.from = comebackModeV3;
                tp.to = normalModeV3;
                isNormalMode = true;
            }
            tp.enabled = true;
        }
    }
}
