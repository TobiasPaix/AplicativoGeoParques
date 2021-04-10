using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class ModelInput : MonoBehaviour
{
    [SerializeField]
    ModelController modelControllerInput;

    /* Esse método é chamado ao se clicar o botão de ação. Mudamos o valor da variável doActionIsPressed
    *  para seu inverso para representar um clique do botão. Ela começa no falso. É uma flag para reprodução
    *  dos estados da animação.
    */
    public void OnClickActionButton(){
        if(modelControllerInput.doActionIsPressed == true){
            modelControllerInput.doActionIsPressed = false;
        }
        else{
            modelControllerInput.doActionIsPressed = true;
        }
    }

    /**
    *   Se o botão especificado para movimento for pressionado, então é verdadeiro que está pressionado.
    *   Usado para começar o movimento. [Depredado]
    */
    public void OnPressActionButton(){
        modelControllerInput.doActionIsPressed = true;
    }

    /**
    *   Se o botão especificado para movimento deixar de ser pressionado, então é falso que está pressionado.
    *   Usado para parar o movimento. [Depredado]
    */
    public void OnReleaseActionButton(){
        modelControllerInput.doActionIsPressed = false;
    }


}
