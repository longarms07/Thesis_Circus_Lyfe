using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnTapSceneChange : MonoBehaviour, ITapListener
{
    public string SceneName = "";

    // Start is called before the first frame update
    void Start()
    {

        IInputManager t = IInputManager.getInstance();
        if (t == null) Destroy(this);
        t.SubscribeTapListener(this, 0);
    }

    public void TapDetected(Vector3 position)
    {
        SceneManager.LoadScene(SceneName);
    }

}
