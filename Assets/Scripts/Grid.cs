using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{

    public int posX;
    public int posY;
    public bool isHinder;
    public Action OnClick;

    //计算预估路径长度三个值
    public int G = 0;
    public int H = 0;
    public int All = 0;

    //记录在寻路过程中该格子的父格子
    public Grid parentGrid;
    public void ChangeColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    //委托绑定模板点击事件
    private void OnMouseDown()
    {
        OnClick?.Invoke();
    }

}
