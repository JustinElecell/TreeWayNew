using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TweenTools
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class TweenAlpha : TweenBase
    {
        [Range(0f, 1f)] public float from = 1f;
        [Range(0f, 1f)] public float to = 1f;
        
        private MaskableGraphic graphic;
        private Color color;

        /// <summary>
        /// 是否已經初始化執行過 Awake
        /// </summary>
        private bool awakeReady = false;

        [Header("物件參考模式, 依據參考的物件的Alpha 值進行初始化, 如果此物件為空, 則依據from to設定值初始化")]
        public bool isRefAlpha = false;

        [Header("當物件完成漸變動作之後的停留時間, 之後會做倒退動作, 如果值小於0 表示關閉此設定")]
        public float showTime = -1;

        [Header("觀察用, 演出計時")]
        public float showTimeCur = 0;
        [Header("觀察用, 到達定點開始計數")]
        public bool showTimeIdle = false;
        [Header("觀察用, 自動倒退動作")]
        public bool showTimeRevMode = false;

        //動作結束時關閉物件
        [Header("動作結束時關閉漸變腳本")]
        public bool endSetDisable = false;

        //動作結束時關閉物件
        [Header("當圖片完全透明時, 關閉圖片物件避免遮擋")]
        public bool isAlphaZeroDisable = false;

        //把 from to 對調進行平移的模式
        public bool reverseMode = false;

        [Header("觀察用, 輸入值無效, 動作起始位置")]
        public float fromRef = 0;
        [Header("觀察用, 輸入值無效, 動作結束位置")]
        public float toRef = 1;

        public void Awake()
        {
            if (awakeReady == true)
            {
                return; 
            }

            //Debug.Log("go.name: " + gameObject.name + " , from = " + from + " , to = " + to + "  AwakeStart ");
            //Debug.Log("go.name: " + gameObject.name + " , fromRef = " + fromRef + " , toRef = " + toRef +  "  AwakeStart ");

            graphic = GetComponent<MaskableGraphic>();
            color = graphic.color;

            //使用物件初始值作為 from 初始值
            if (isRefAlpha == true)
            {
                from = fromRef = color.a;
                to = toRef = 1;
                //Debug.Log("go.name: " + gameObject.name + " , fromRef = " + fromRef + " , toRef = " + toRef + " isRefAlpha");
            }
            //指定初始值
            else
            {
                fromRef = from;
                toRef = to;
                //Debug.Log("go.name: " + gameObject.name + " , fromRef = " + fromRef + " , toRef = " + toRef);
            }


            color.a = from;
            graphic.color = color;
            awakeReady = true;
        }

        public override void OnInit()
        {
            if (graphic == null)
            {
                graphic = GetComponent<MaskableGraphic>();
            }
            color = graphic.color;
            color.a = from;
            graphic.color = color;

        }

        public void TranToReverseMode(bool _isReverseMode)
        {
            if (awakeReady == false)
            {
                Awake();
            }

            reverseMode = _isReverseMode;

            //Debug.Log("go.name = " + gameObject.name + " , TranToReverseMode = " + reverseMode);
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

        //原版
        //public override void OnUpdate(float value)
        //{
        //    color.a = from + value * (to - from);
        //    graphic.color = color;
        //}


        public override void OnUpdate(float value)
        {
            color.a = from + value * (to - from);
            graphic.color = color;


            graphic.enabled = true;
            if (isAlphaZeroDisable == true)
            {
                if (color.a < 0.01f)
                {
                    //Debug.Log("isAlphaZeroDisable == true");
                    graphic.enabled = false;
                }
            }

            if (showTime > 0 && reverseMode == false)
            {
                //是否到達定點
                if (value > 0.999f)
                {

                    showTimeIdle = true;
                    StartCoroutine(ShowTimeCount());
                }
            }
            else if (showTime > 0 && reverseMode == true && showTimeIdle == true)
            {
                //是否到達定點
                if (value > 0.999f)
                {

                    showTimeIdle = false;
                    TranToReverseMode(false);
                    enabled = false;
                }
            }
            else
            {
                if (endSetDisable && value > 0.999f)
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
    }
}