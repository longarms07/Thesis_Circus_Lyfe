using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDayTimeChangeListener 
{
    void DayTimeChange(DayEnums newDay, TimeEnums newTime);
    void PerformanceDay();
}
