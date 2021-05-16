using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
    /*******************************************************************************************/
    /**************************** Referência para cada script separado *************************/
    /*******************************************************************************************/
    [SerializeField] 
    internal ModelInput modelInput;

    [SerializeField] 
    internal ModelPlacement modelPlacement;

    [SerializeField] 
    internal ModelMovement modelMovement;

    [SerializeField] 
    internal ModelAnimation modelAnimation;

    [SerializeField]
    internal SceneController scController;

    /*******************************************************************************************/
    /********************************* Propriedades do modelo **********************************/
    
     /*
    *   Instância do prefab.
    */
    
    internal GameObject modelInstance;

    /*
    *   Prefab a ser instanciado pelo script.
    */ 
    public GameObject modelPrefab;

    /*
    *   Câmera traseira em primeira pessoa para poder ver o mundo real.
    */
    public Camera firstPersonCamera;

    /*
    *   Plano detectado pelo controlador da cena e passado para essa classe. É o local onde o 
    *   modelo 3D será instanciado.
    */
    internal DetectedPlane detectedPlane;

    /*
    *   Controla as animações, recebe do próprio prefab instanciado. Usado em ModelAnimation.
    */
    internal Animator anim;
    /* 
    *   Corpo Rígido do modelo, recebe do próprio prefab instanciado. Usado em ModelMovement. 
    */
    internal Rigidbody rb;
    /* 
    *   Verifica se o botão de movimento está pressionado. Usado em ModelInput.
    */
    public bool doActionIsPressed;
    /* 
    *   Verifica se o modelo está se movendo. Usado em ModelMovement e em ModelAnimation.
    */
    internal bool isMoving;
    /* 
    *   Verifica se o modelo está se movendo. Usado em ModelMovement e em ModelAnimation.
    */
    internal bool isSitting;
    
    private string currentState;
    

    /*******************************************************************************************
    *   Recebe o DetectedPlane do controlador da cena, e chama o método para criar uma instância 
    *   do modelo.
    ********************************************************************************************/
    public void SetTrackableHit(TrackableHit trackableHit){
        modelPlacement.SpawnInstance(trackableHit);
        modelMovement.SetTrackableHit(trackableHit);
    }

    /*******************************************************************************************
    *   Muda o estado da animação, dependendo da string.
    ********************************************************************************************/
    public void ChangeState(string newState){
        if(newState != currentState){
            anim.Play(newState);
            currentState = newState;
        }
    }


    /*******************************************************************************************
    *   Esse método é chamado quando o botão Recomeçar do Menu de configurações é apertado.
    *   Destrói a instância do modelo, para que se possa colocá-lo em outro lugar, ou simplesmente
    *   limpar o plano detectado.
    ********************************************************************************************/
    public void RestartButtonSettings(){
        if(modelInstance == null){
            scController.Canvas.transform.GetChild(0).gameObject.SetActive(false);
            scController.Canvas.transform.GetChild(3).gameObject.SetActive(false);

            return;
        }
        else{
            Destroy(modelInstance, 0.05f);
        }
    }
}
