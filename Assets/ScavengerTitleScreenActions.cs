using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScavengerTitleScreenActions : MonoBehaviour
{
    public void StartScavengerDemo()
    { SceneManager.LoadScene("SpaceShip"); }
    private void FixedUpdate()
    {
        Camera.main.transform.Rotate(0.2f, 0.2f, 0);
        if ((Input.GetAxis("Submit") > 0.5f) || (Input.GetAxis("Fire3") > 0.5f) || (Input.GetAxis("Fire1") > 0.5f))
        { StartScavengerDemo(); }
        if ((Input.GetAxis("Fire2") > 0.5f))
        { ShowCredits(); }
        if ((Input.GetAxis("Fire3") > 0.5f))
        { Application.Quit(); }
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("ScavengerCredits");
    }
}
