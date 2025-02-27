using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private AssetLabelReference obj;

    public void LoadSceneMenu()
    {
        Addressables.LoadSceneAsync("Assets/SlimUI/Modern Menu 1/Scenes/Demos/MainCopy.unity");
    }

}
