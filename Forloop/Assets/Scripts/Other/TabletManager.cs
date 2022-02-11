using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletManager : MonoBehaviour
{
    public static TabletManager instance;

    private Animator _animator;

    public void CloseTablet()
    {
        _animator.SetTrigger("Close");
    }

    public void OnCloseAnimationCompleted()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
        _animator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }
}
