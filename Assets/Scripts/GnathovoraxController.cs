using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class GnathovoraxController : MonoBehaviour
{
    /// <summary>
    /// aaa
    /// <\summary>
    private DetectedPlane detectedPlane;

    /**
    *  aaa
    **/
    bool isWalking = true;

    /**
    *  Animator que contém as animações do modelo. 
    **/
    private Animator animController;

    /**
    *  Botão que controla se o modelo irá se mover para o ponteiro ou não.
    **/
    public DoMoveButtonHandler doMoveButton;

    /**
    *  Prefab a ser instanciado pelo script.
    **/
    public GameObject gnathoPrefab;


    private GameObject gnathoInstance;


    public GameObject pointer;

 
    public Camera firstPersonCamera;

    // Velocidade para se mover
    public float speed = 20.0f;

    

    // Start is called before the first frame update
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gnathoInstance == null || gnathoInstance.activeSelf == false){
            pointer.SetActive(false);
            return;
        }
        else{
            pointer.SetActive(true);
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if (Frame.Raycast(Screen.width/2, Screen.height/2, raycastFilter, out hit)){
            Vector3 pt = hit.Pose.position;
            
            pt.y = gnathoInstance.transform.position.y;

            Vector3 pos = pointer.transform.position;
            pos.y = pt.y;
            pointer.transform.position = pos;

            pointer.transform.position = Vector3.Lerp(pointer.transform.position, pt, Time.smoothDeltaTime*speed);
        }
    }

    private void FixedUpdate() {
        if(doMoveButton.isPressed){
            setWalkAnimation(isWalking);
            MoveToPointer();
        }
        else{
            setWalkAnimation(!isWalking);
        }
    }

    public void SetPlane(DetectedPlane plane){
        detectedPlane = plane;
        // Spawn a new gnathovorax.
        SpawnGnatho();
    }

    void SpawnGnatho(){
        if(gnathoInstance != null){
            Destroy(gnathoInstance, 0.01f);
        }

        Vector3 pos = detectedPlane.CenterPose.position;

        gnathoInstance = Instantiate(gnathoPrefab, pos, Quaternion.identity, transform);
        animController = gnathoInstance.GetComponentInChildren<Animator>();
        setWalkAnimation(!isWalking);
    }

    public void MoveToPointer(){
        // Mover-se na direção do pointer, diminuir velocidade se muito perto.
        float dist = Vector3.Distance(pointer.transform.position, gnathoInstance.transform.position) - 0.2f;
        if(dist < 0){
            dist = 0;
            // Se estiver muito perto do ponteiro, então a animação de caminhar não é verdadeira.
            setWalkAnimation(!isWalking);
        }
        else{
            // Para diminuir a velocidade da animação conforme estiver perto de parar.
            if(dist < 0.5){
                animController.SetFloat("walkSpeed", dist + 0.4f);
            }
            else{
                animController.SetFloat("walkSpeed", 1.0f);
            }
            setWalkAnimation(isWalking);
        }
        
        Rigidbody rb = gnathoInstance.GetComponent<Rigidbody>();
        rb.transform.LookAt(pointer.transform.position);
        rb.velocity = gnathoInstance.transform.localScale.x * gnathoInstance.transform.forward * dist/ .5f;
    }

    public void setWalkAnimation(bool value){
        animController.SetBool("isWalking", value);
    }
}
