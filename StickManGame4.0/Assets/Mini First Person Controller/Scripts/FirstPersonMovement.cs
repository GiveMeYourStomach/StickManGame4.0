using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleKeyInputJump : MonoBehaviour
{
    public Animator characterAnimator;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterAnimator.SetTrigger("JumpTrigger");
        }
    }
}