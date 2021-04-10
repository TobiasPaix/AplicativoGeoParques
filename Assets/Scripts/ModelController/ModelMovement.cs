using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ModelMovement : MonoBehaviour
{
    [SerializeField] 
    ModelController modelControllerMov;

    public GameObject pointer;
    public FixedJoystick fixedJoystick;
    internal float x, y;
    internal Vector3 hitPosition;
    /* Esse é o raio ao redor do modelo que, se o ponteiro estiver dentro, o modelo não irá se mover.*/
    [Range(0.0f, 0.9f)] public float distThreshold = 0.2f;
    /* Velocidade a ser adicionada na força para mover a instância, e também para mover o ponteiro (quando havia um).*/  
    [Range(1.0f, 30.0f)] public float pointerSpeed = 20.0f;

    // Update is called once per frame
    void Update()
    {
        //SetPointerActive(modelControllerMov.modelInstance);
    }


    void FixedUpdate(){
        if(modelControllerMov.modelInstance == null){
            return;
        } 
        else{
            /* Se for verdade que o botão foi pressionado, então ativa o movimento e chama a animação.*/
            MoveWithJoystick(modelControllerMov.modelInstance);
        }
        
    }

    /**
    *   Esse método move o modelo de acordo com o fixedJoystick. Verifica se a posição X e Y
    *   do Joystick estão fora do centro. Se for o caso, então aplicamos uma rotação em graus 
    *   em torno do eixo Y com a expressão Mathf.Atan2(x, y)*Mathf.Rad2Deg. 
    *   Ativamos a variável que denota o estado de movimento do modelo se o Joystick está 
    *   em qualquer lugar diferente do centro (o modelo está se movendo).
    *   
    *   Joystick.Horizontal e Joystick.Vertical estão ambos dentro do limite [-1, 1].
    */
    void MoveWithJoystick(GameObject modelInstance){
        x = fixedJoystick.Horizontal;
        y = fixedJoystick.Vertical;

        // /* Referência ao ponto onde houve um toque na tela que encontrou um plano detectado.*/
        // TrackableHit hitHalfScreen;
        // /* Aceita somente toque se estiver em um plano dentro dos limites.*/
        // TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        // if (Frame.Raycast(Screen.width/2, Screen.height/2, raycastFilter, out hitHalfScreen)){
        //      Vector3 screenPoint = hitHalfScreen.Pose.position.normalized;
        //     Vector3 instancePoint = modelInstance.transform.position.normalized;


        //     Vector3 diffModelScreen = screenPoint - instancePoint;

        //     Vector3 ver = new Vector3 (diffModelScreen.x, 0, diffModelScreen.z);

        //     /* Álgebra Linear para obter o vetor perpendicular a esse, para agir como o eixo horizontal.*/
        //     /* Rotação em -90°: (Podemos desconsiderar o Y, pois o plano é delimitado por X e Z.)
        //     *  |  0 0 1  |    | X |
        //     *  |  0 0 0  | X  | Y |
        //     *  | -1 0 0  |    | Z |
        //     */

        //     Vector3 hor = new Vector3(ver.z, 0, -ver.x);

            
        //     Vector3 direction = x * hor + y * ver;
            Vector3 direction = x * Vector3.right + y * Vector3.forward;
            modelControllerMov.rb = modelInstance.GetComponent<Rigidbody>();
            
            modelControllerMov.rb.AddForce(direction * pointerSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);

            /* Rotaciona o modelo de acordo com a direção que está indo, e ativa o estado isMoving.*/
            if(x != 0 && y != 0){
                /* Para a rotação Y, precisamos dela em graus.*/
                Vector3 instanceEulerAngles = modelControllerMov.modelInstance.transform.eulerAngles;
                instanceEulerAngles = new Vector3(instanceEulerAngles.x, Mathf.Atan2(x, y)*Mathf.Rad2Deg, instanceEulerAngles.z);
                modelControllerMov.modelInstance.transform.eulerAngles = instanceEulerAngles;
                modelControllerMov.isMoving = true;
            }
            else if(x == 0 && y != 0){
                modelControllerMov.isMoving = true;
            }
            else if(x != 0 && y == 0){
                modelControllerMov.isMoving = true;
            }
            else{
                modelControllerMov.isMoving = false;
            }
        // }
    } 


    /** 
    *   
    *   <param name="hit"> O ponto onde foi acertado o raycast no plano AR.</param>
    */
    public void SetTrackableHit(TrackableHit hit){
        hitPosition = hit.Pose.position;
    }

    /**
    *   Esse método primeiro verifica se há uma instância do modelo na cena. Se não houver, não há motivos 
    *   para o ponteiro aparecer, então fica desativado.
    *   Caso contrário, ativa ele e faz com que apareça na altura (model.y) do modelo, e no centro da tela.
    *   [Depredado]: Não está mais em uso, porém deixo aqui caso queira se usar de novo o movimento antigo,
    *   o de seguir o ponteiro ao apertar o botão.
    *   <param name="modelInstance"> Instância do modelo.</param>
    */
    void SetPointerActive(GameObject modelInstance){
        if(modelInstance == null || modelInstance.activeSelf == false){
            pointer.SetActive(false);
            return;
        }
        else{
            pointer.SetActive(true);
        }

        /* Referência ao ponto onde houve um toque na tela que encontrou um plano detectado.*/
        TrackableHit hit;
        /* Aceita somente toque se estiver em um plano dentro dos limites.*/
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if (Frame.Raycast(Screen.width/2, Screen.height/2, raycastFilter, out hit)){
            /* Armazena a posição do encontro entre o toque e o plano.*/
            Vector3 pt = hit.Pose.position;
            
            /* Substitui a altura do vetor acima com a altura da instância.*/
            pt.y = modelInstance.transform.position.y;

            /* Armazena a posição do próprio ponteiro*/
            Vector3 pos = pointer.transform.position;
            /* Torna ela igual à altura da instância.*/
            pos.y = pt.y;
            /* Atualiza a posição do ponteiro com a altura modificada.*/
            pointer.transform.position = pos;

            /* Realiza um Lerp (para mover o ponteiro para o ponto do TrackableHit de forma linear) 
            *  entre a posição atual do ponteiro e o TrackableHit, de forma que ele persegue o centro 
            *  da tela onde há TrackableHits acontecendo.  
            */
            pointer.transform.position = Vector3.Lerp(pointer.transform.position, pt, Time.smoothDeltaTime*pointerSpeed);
        }
    }

    /**
    *   Move o modelo na direção do ponteiro, em que a distância entre ambos dita a velocidade que o 
    *   modelo irá se mover -- quanto maior a distância, maior é a velocidade para se encontrarem. Há
    *   um raio em volta do modelo com valor igual a distThreshold, o que significa que se a distância
    *   for menor que esse valor, o modelo não irá se mover.
    *   [Depredado]: Usado em conjunto com SetPointerActive, ou após ativar o ponteiro, porém era o 
    *   sistema antigo de movimento.
    */
    void MoveToPointer(){
        /* Move o modelo na direção do ponteiro, diminui a velocidade se muito perto.*/
        float dist = Vector3.Distance(pointer.transform.position, (modelControllerMov.modelInstance).transform.position) - distThreshold;
        if(dist < 0){
            dist = 0;
            modelControllerMov.modelAnimation.StateIdle();
            modelControllerMov.isMoving = false;
        }
        else{
            modelControllerMov.isMoving = true;
        }
        
        modelControllerMov.rb = modelControllerMov.modelInstance.GetComponent<Rigidbody>();
        modelControllerMov.rb.transform.LookAt(pointer.transform.position);
        /* Divide por um valor pequeno para aumentar a velocidade.*/
        if(dist > 2.0f){
            modelControllerMov.rb.velocity = (modelControllerMov.modelInstance).transform.localScale.x * (modelControllerMov.modelInstance).transform.forward * dist/ .5f;
        }
        else{
            modelControllerMov.rb.velocity = (modelControllerMov.modelInstance).transform.localScale.x * (modelControllerMov.modelInstance).transform.forward * dist/ .01f;
        }
        
    }
}
