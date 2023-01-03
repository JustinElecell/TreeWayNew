using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //   1)Smooting表示停止滑动后，当前页码归正的速率
    //   2）sensitivity 滑动的敏感度，如果数值过大会导致翻多页
    //   3)OnPageChanged 当前页码改变时回调
    //   4)方法pageTo 直接跳转到某一页
    //注意点：ScrollView下的Content的长度是每页的宽度* 页数，每页的宽度与ScrollView的宽度相同

    private ScrollRect rect;    //滑动组件 
    private float targethorizontal = 0;     //滑动的起始坐标          
    private bool isDrag = false;                 //是否拖拽结束  
    private List<float> posList = new List<float>();     //求出每页的临界值（0-1）
    //private int currentPageIndex = -1;  //记录当前是第几页，页索引从0开始 ，这里不需要显示，有需求可以自己显示
    //public Action<int> OnPageChanged;

    private bool stopMove = true;   //是否停止移动
    public float smooting = 4;      //滑动速度  
    public float sensitivity = 0;   //灵敏度
    private float startTime;        //从开始拖动到结束的时间

    private float startDragHorizontal;//记录当前开始滑动的位置（0-1）


    public void Init()
    {
        rect = transform.GetComponent<ScrollRect>();
        posList.Add(0);

        for (int i = 1; i < rect.content.transform.childCount - 1; i++)
        {//求出每页的临界值（0-1）
            var tmp = ((1f / (rect.content.transform.childCount - 1)) * (float)i);
            posList.Add(tmp);
            //print(GetComponent<RectTransform>().rect.width * i / horizontalLength);
        }
        posList.Add(1);
    }

    void Update()
    {
        if (!isDrag && !stopMove)//如果拖动没有结束并且界面还没停止移动就继续移动
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            //水平滚动位置，以 0 到 1 之间的值表示，0 表示位于左侧。用lerp可以实现平缓过渡
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, t);
            if (t >= 1)
                stopMove = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        startDragHorizontal = rect.horizontalNormalizedPosition;//开始滑动位置（0-1）
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = rect.horizontalNormalizedPosition;//结束拖拽的位置（0-1）
        posX += ((posX - startDragHorizontal) * sensitivity);//拖拽距离乘以灵敏度
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        //计算当前拖拽到的位置到哪个界面最近就显示哪个界面
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posX);
        for (int i = 1; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        //SetPageIndex(index);
        targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值  
        isDrag = false;
        startTime = 0;
        stopMove = false;
    }


}



