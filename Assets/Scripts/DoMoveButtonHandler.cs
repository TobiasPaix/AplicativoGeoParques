using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMoveButtonHandler : MonoBehaviour
{
    public bool isPressed;

    public void Update(){
        if(isPressed){
            Debug.Log("Is pressed.");
        }
            
    }

    public void onPress(){
        isPressed = true;
    }

    public void onRelease(){
        isPressed = false;
    }
        
}
