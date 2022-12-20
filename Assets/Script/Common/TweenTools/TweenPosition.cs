using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TweenTools
{
    /// <summary>
    /// 製作移動動畫時使用, 可代替 NGUI 的 TweenPosition
    /// </summary>
    [RequireComponent(typeof(MaskableGraphic))]
    public class TweenPosition : TweenBase
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;

        [Header("物件參考模式, 依據參考的物件位置做位移, 可用於自適應")]
        public bool isGoRefPosition = false;

        /// <summary>
        /// 不變的參數, 紀錄起始位置, 不受函數 TranToReverseMode 影響
        /// </summary>
        [Header("紀錄起始位置")]
        public RectTransform fromRTRef;
        /// <summary>
        /// 不變的參數, 紀錄結束位置, 不受函數 TranToReverseMode 影響
        /// </summary>
        [Header("紀錄結束位置")]
        public RectTransform toRTRef;

        [Header("當物件出現後, 完成動作之後的停留時間, 之後會做倒退動作, 如果值小於0 表示關閉此設定")]
        public float showTime = -1;

        [Header("觀察用, 演出計時")]
        public float showTimeCur = 0;
        [Header("觀察用, 到達定點開始計數")]
        public bool showTimeIdle = false;
        [Header("觀察用, 自動倒退動作")]
        public bool showTimeRevMode = false;

        //動作結束時關閉物件
        [Header("動作結束時關閉物件")]
        public bool endSetDisable = false;
        private MaskableGraphic graphic;
        [Header("觀察用, 輸入值無效")]
        public Vector3 position;


        //把 from to 對調進行平移的模式
        private bool reverseMode = false;

        [Header("觀察用, 輸入值無效, 動作起始位置")]
        public Vector3 fromRef = Vector3.zero;
        [Header("觀察用, 輸入值無效, 動作結束位置")]
        public Vector3 toRef = Vector3.zero;

        public void Awake()
        {
            //此事件就算腳本是關閉的, 只要物件本身是開啟的就會執行
            //Debug.LogError(gameObject.name + "  TP Awake");

            if (isGoRefPosition)
            {
                //Debug.LogError("fromRTRef.localPosition = " + fromRTRef.localPosition);
                from = fromRef = fromRTRef.localPosition;
                //Debug.LogError("toRTRef.localPosition = " + toRTRef.localPosition);
                to = toRef = toRTRef.localPosition;

            }
            else
            {
                fromRef = from;
                toRef = to;
            }

            gameObject.transform.localPosition = from;
        }

        public override void OnInit()
        {
            graphic = GetComponent<MaskableGraphic>();
            position = graphic.rectTransform.localPosition = from;

        }

    

        public void TranToReverseMode(bool _isReverseMode)
        {
            reverseMode = _isReverseMode;

            if (reverseMode)
            {
                from = toRef;
                to = fromRef;
            }
            else
            {
                from = fromRef;
                to = toRef;
            }
        }


        public override void OnUpdate(float value)
        {
            position = from + value * (to - from);
            //position.x = from.x + value * (to.x - from.x);
            //position.y = from.y + value * (to.y - from.y);
            //position.z = from.z + value * (to.z - from.z);
            graphic.rectTransform.localPosition = position;

            //if (fromRef != fromRTRef.localPosition)
            //{
            //    Debug.LogError("fromRTRef.localPosition = " + fromRTRef.localPosition);
            //    fromRef = fromRTRef.localPosition;
            //    Debug.LogError("toRTRef.localPosition = " + toRTRef.localPosition);
            //    toRef = toRTRef.localPosition;
            //}

            if (showTime > 0 && reverseMode == false)
            {
                //是否到達定點
                if (value > 0.99f)
                {
                    showTimeIdle = true;
                    StartCoroutine(ShowTimeCount());
                }
            }
            else if (showTime > 0 && reverseMode == true && showTimeIdle == true)
            {
                //是否到達定點
                if (value > 0.99f)
                {
                    showTimeIdle = false;
                    TranToReverseMode(false);
                    enabled = false;
                }
            }
            else
            {
                if (endSetDisable && value > 0.99f)
                {
                    enabled = false;
                }
            }
        }

        IEnumerator ShowTimeCount()
        {
            yield return new WaitForSeconds(showTime);

            showTimeCur = 0;
            TranToReverseMode(true);
            enabled = false;
            enabled = true;
        }

        //public new void ResetToBeginning()
        //{
        //    //mStarted = false;
        //    //mFactor = (amountPerDelta < 0f) ? 1f : 0f;
        //    //Sample(mFactor, false);
        //    base.ResetToBeginning();
        //    graphic.rectTransform.localPosition = from;

        //}

        //public void OnEnable()
        //{
        //    TranToReverseMode(reverseMode);
        //}

        public void DisableGO()
        {
            gameObject.SetActive(false);
        }
    }

}