using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P1HP : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHP(float HP)
    {
        slider.maxValue = HP;
        slider.value = HP;
    }

    public void SetHP(float HP)
    {
        slider.value = HP;
    }
}
