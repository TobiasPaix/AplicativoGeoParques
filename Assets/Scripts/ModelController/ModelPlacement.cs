using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ModelPlacement : MonoBehaviour
{

   [SerializeField]
   ModelController modelControllerPlac;
   public float _instanceRotation = 180.0f;

    /**
    *   Método para instanciar o prefab do modelo. Recebe o parâmetro DetectedPlane para saber onde colocar
    *   a instância do modelo.
    */
    internal void SpawnInstance(TrackableHit hit){
        if(modelControllerPlac.modelInstance != null){
            Destroy(modelControllerPlac.modelInstance, 0.01f);
        }
        
        modelControllerPlac.modelInstance = Instantiate(modelControllerPlac.modelPrefab, hit.Pose.position, hit.Pose.rotation);
        modelControllerPlac.modelInstance.transform.Rotate(0, _instanceRotation, 0, Space.Self);

        /* Inicializa o controlador de animação do modelo.*/
        modelControllerPlac.anim = modelControllerPlac.modelInstance.GetComponentInChildren<Animator>();
        /* Inicializa as animações e os estados da mesma.*/
        modelControllerPlac.modelAnimation.StateIdle();
        modelControllerPlac.isSitting = false;
        modelControllerPlac.isMoving = false;
    }
}
