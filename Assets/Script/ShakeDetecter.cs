using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeDetecter {

    public delegate void ShakeEvent(string shakedItemType, int ItemIndex, float unit);
    public static event ShakeEvent Shaked;

    public static void makeShakedEvent(string shakedItemType, int ItemIndex, float unit)
    {
        Shaked(shakedItemType, ItemIndex, unit);
    }
}
