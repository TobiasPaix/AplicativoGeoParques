using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
// Para poder herdar de Manipulator.
using GoogleARCore.Examples.ObjectManipulation; 

public class ObjectGenerator : Manipulator
{

    /// <summary>
    /// A câmera em primeira pessoa usada para renderizar a imagem
    /// do ARCore, a imagem de fundo do ARCore.
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// Um prefab para colocar quando um raycast de toque d@ usuári@
    /// acerta um plano.
    /// </summary>
    public GameObject ObjectPrefab;

    /// <summary>
    /// Prefab do manipulador para anexar a objetos no plano. 
    /// </summary>
    public GameObject ManipulatorPrefab;

    /// <summary>
    /// Variável estática para controlar o número máximo de prefabs na cena.
    ///</summary>
    public static int numPrefab;

    /// <summary>
    /// Função herdada de Manipulator. Retorna verdadeiro se a 
    /// manipulação pode ser começada para um determinado gesto.
    /// Como é herdada, iremos fazer um override nela. Vai valer 
    /// a do objeto filho.
    /// </summary>
    /// <param name="gesture">O gesto atual.</param>
    /// <returns>Verdadeiro se a manipulação pode começar.</returns>
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null){
            return true;
        }

        return false;
    }

    ///<summary>
    /// Função chamada quando a manipulação é terminada.
    /// </summary>
    /// <param name="gesture">O gesto atual.</param>
    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled){
            return;
        }

        // Se o gesto tem como alvo um objeto existente,
        // então terminamos.
        if (gesture.TargetObject != null){
            return;
        }

        // Raycast sobre a localidade que a pessoa tocou para 
        // procurar por planos.
        TrackableHit hit;
        // Só aceita planos que condizam com PlaneWithinPolygon
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(
            gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
        {
            // Usar a pose do acerto e da câmera para verificar se o acerto foi
            // na parte de trás do plano, e se for, não precisa criar uma âncora.
            if ((hit.Trackable is DetectedPlane) && 
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Acerto na parte de trás do DetectedPlane atual");
            }
            else
            {
                ///if (numPrefab < 1){
                    ///numPrefab++;
                    /// Instanciar o objeto na pose do acerto.
                    var gameObject = Instantiate(ObjectPrefab, hit.Pose.position, hit.Pose.rotation);

                    /// Instanciar o manipulador
                    var manipulator = 
                        Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                    /// Torna o objeto um filho do manipulador.
                    gameObject.transform.parent = manipulator.transform;

                    /// Criar uma âncora para permitir o ARCore rastrear o ponto de acerto
                    /// conforme seu entendimento do mundo real evolui.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    /// Torna o manipulador um filho da âncora.
                    manipulator.transform.parent = anchor.transform;

                    /// Selecionar o objeto recém colocado.
                    manipulator.GetComponent<Manipulator>().Select();
                ///}
                
                
            }

        }
    }
}
