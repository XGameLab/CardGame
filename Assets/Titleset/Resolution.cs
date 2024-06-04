using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    //Dropdownを格納する変数
    [SerializeField] private Dropdown dropdown;
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dropdown.value == 0)
        {
            OnClick_1920x1080();
        }
        //DropdownのValueが1のとき（青が選択されているとき）
        else if(dropdown.value == 1)
        {
            OnClick_1366x768();
        }
        //DropdownのValueが2のとき（黄が選択されているとき）
        else if (dropdown.value == 2)
        {
            OnClick_2560x1440();
        }
        //DropdownのValueが3のとき（白が選択されているとき）
        else if (dropdown.value == 3)
        {
             OnClick_3840x2160();
        }
        
    }


    public void OnClick_1920x1080()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 60);
    }

    public void OnClick_1366x768()
    {
        Screen.SetResolution(1366, 768, FullScreenMode.Windowed, 60);
    }

     public void OnClick_2560x1440()
    {
        Screen.SetResolution(2560, 1440, FullScreenMode.Windowed, 60);
    }

    public void OnClick_3840x2160()
    {
        Screen.SetResolution(3840, 2160, FullScreenMode.Windowed, 60);
    }
}
