using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VersionExclusiveText : MonoBehaviour
{

    TextMeshProUGUI tmp;
    public string WebglText;
    public string AndroidText;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (Application.platform == RuntimePlatform.WebGLPlayer) tmp.text = WebglText;
        else if (Application.platform == RuntimePlatform.Android) tmp.text = AndroidText;
    }
}
