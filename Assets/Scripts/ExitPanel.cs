using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que se encarga de las acciones del panel de exit
public class ExitPanel : MonoBehaviour
{
    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnCancelButton()
    {
        GetComponent<Animator>().SetBool("Opened", false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Animator>().SetBool("Opened", true);
        }
    }
}
