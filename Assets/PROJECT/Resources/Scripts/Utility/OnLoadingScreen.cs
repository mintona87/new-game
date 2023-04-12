using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLoadingScreen : MonoBehaviour
{
    public static OnLoadingScreen Instance;

    public bool onlyOnceDisableLoadPanel;

    CanvasGroup canvasGroup;

    void Start()
    {
        Instance = this;
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();

        SetLoadingScreenActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoadingScreenActive(bool condition)
    {
        ActiveAnimation(condition);
    }

    void ActiveAnimation(bool condition)
    {
        if (condition)
        {
            canvasGroup.alpha = 1.0f;
            gameObject.SetActive(condition);
            onlyOnceDisableLoadPanel = false;
        }
        else
        {
            if (!onlyOnceDisableLoadPanel)
            {
                
                StartCoroutine(DisableModal(condition));
            }
        }
    }
    IEnumerator DisableModal(bool condition)
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(condition);
        onlyOnceDisableLoadPanel = true;
    }
}
