using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boss : MonoBehaviour
{
    public Stetas stetas;

    Dictionary<int, Action<int>> AttackFunc = new Dictionary<int, Action<int>>();

    int repeatTimes;

    private void Start()
    {
        AttackFuncInit();
    }


    // 攻擊方式
    void AttackFuncInit()
    {

        // 在所處路線發射主子彈
        AttackFunc.Add(1, (x) => { 
        
        
        
        });

        // 向隨機 x 條路線同時發射一次主子彈
        AttackFunc.Add(3, (x) => { 
        
        
        });

        // 向自身所在路線發射主子彈，並向另外兩條路線同時發射指定編號子彈
        AttackFunc.Add(4, (x) => { 
        
        
        });

    }




}
