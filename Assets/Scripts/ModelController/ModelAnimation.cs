using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ModelAnimation : MonoBehaviour
{
    [SerializeField]
    ModelController modelControllerAnim;

    private FixedJoystick fixedJoystickAnim;
    private float velocity;

    public float idleTime = 0.0f;
    /* Nome dos estados no Animation Controller.*/
    const string STATE_IDLE = "GnathoIdle"; // Anteriormente "WolfIdle"
    const string STATE_CREEP = "GnathoWalk"; // Anteriormente "WolfCreep"
    const string STATE_WALK = "GnathoWalk"; // Anteriormente "WolfWalk"
    const string STATE_RUN = "GnathoWalk"; // Anteriormente "WolfRun"
    const string STATE_SIT = "GnathoIdle"; // Anteriormente "WolfSit"
    const float USE_WALK = 0.8f; // Velocidade mínima para a animação a executar ser WolfWalk.
    const float USE_CREEP = 0.7f;  // Velocidade mínima para a animação a executar ser WolfCreep.
    const float SECS_PER_FRAME = 0.02f; // Duração de 1 frame.

    /**
    *   Atualizar a física do corpo rígido, e assim as animações referentes à instância.
    */
    private void Update() {
        /* Se não houver instância do prefab, não há o que animar.*/
        if(modelControllerAnim.modelInstance == null){
            return;
        }
        else{
            ControlStates();
        }
    }

    /**
    *   O método ControlStates fornece uma lógica para a animação. Ao setar e resetar
    *   as variáveis isSitting e isMoving, podemos saber em qual estado estamos e assim
    *   reproduzir a animação referente.
    */
   public void ControlStates(){
       /* Caso não haja instância, não há animação.*/
       if(modelControllerAnim.modelInstance == null){
           return;
       }
        /* Verifica se o botão para a ação está pressionado.*/
       if(modelControllerAnim.doActionIsPressed){
           /* Se estiver, então realiza a ação de sentar.*/
           StateSit();
           /* E torna o estado isSitting verdadeiro.*/
           modelControllerAnim.isSitting = true;
           if(modelControllerAnim.isMoving){
               modelControllerAnim.doActionIsPressed = false;
           }
       }
       else{
           /* O botão de realizar ação não foi pressionado.*/
           if(!(modelControllerAnim.isMoving)){
               /* Se não há movimento, a animação é Idle. Parado.*/
               StateIdle();
           }
           else{
                /* Se há movimento, então há dois caminhos que podemos tomar.*/
                if(modelControllerAnim.isSitting){
                /* Caso o estado anterior for Sentado, então a animação passa de
                *  Sentado > Rastejar > Andar, dependendo da velocidade do modelo.
                */
                    if(modelControllerAnim.rb.velocity.magnitude < USE_CREEP){
                        StateCreep();
                    }
                    else{
                        StateWalk();
                        /* Saiu do estado Sentado.*/
                        modelControllerAnim.isSitting = false;
                    }
                }
                else{
                    /* Caso o estado anterior for Parado, então a animação passa de
                    *  Parado > Andar > Correr, dependendo da velocidade do modelo.
                    */
                    if(modelControllerAnim.rb.velocity.magnitude < USE_WALK){
                        StateWalk();
                    }
                    else{
                        StateRun();
                    }
                }
           }
       }
   }

    /**
    *   Animação Idle.
    */
    public void StateIdle(){
        modelControllerAnim.ChangeState(STATE_IDLE);
    }

    /**
    *   Animação Walk.
    */
    public void StateWalk(){
        modelControllerAnim.ChangeState(STATE_WALK);
    }

    /**
    *   Animação Run.
    */
    public void StateRun(){
        modelControllerAnim.ChangeState(STATE_RUN);
    }

    /**
    *   Animação Creep.
    */
    public void StateCreep(){
        modelControllerAnim.ChangeState(STATE_CREEP);
    }

    /**
    *   Animação Sit.
    */
    public void StateSit(){
        modelControllerAnim.ChangeState(STATE_SIT);
    }
}
