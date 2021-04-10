//-----------------------------------------------------------------------
// <copyright file="SceneController.cs" company="Google LLC">
//
// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GoogleARCore;
using UnityEngine.UI;

#if UNITY_EDITOR
// Habilita a propagação do toque no celular para o computador enquanto em Instant Preview.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class SceneController : MonoBehaviour
{
    /// <summary>
    /// A câmera em primeira pessoa.
    /// </summary>
    public Camera FirstPersonCamera;

    public ModelController modelController;

    public Button doActionButton;
    public FixedJoystick fixedJoystick;
    public GameObject arCoreDevice;

    public Text debugText;

    private ARCoreSession arSession;
    private ARCoreSessionConfig arSConfig;
  
  
    // Update is called once per frame
    void Update()
    {
        // Caso não houve toques na tela, então terminamos o update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began){
            return;
        }

        // Se a pessoa está em Interface de Usuário, então não recebe input.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
            return;
        }

        // Raycast até a localidade que a pessoa tocou para procurar por planos.
        TrackableHit hit;
        bool foundHit = false;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        foundHit = Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit);
    

        // Se achamos um plano, podemos colocar o prefab no mundo.
        if (foundHit){

            // Com a pose do acerto e da câmera, verificamos se o acerto
            // foi na parte de trás do plano. Se for, então não precisamos
            // criar uma âncora.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Acerto na parte de trás do atual DetectedPlane.");
            }
            else{
                SetTrackableHit(hit);
                TrackableHit hitFromScreen;
                Frame.Raycast(Screen.width/2, Screen.height/2, raycastFilter, out hitFromScreen);
                // debugText.text = arSConfig.ToString();
                doActionButton.gameObject.SetActive(true);
                fixedJoystick.gameObject.SetActive(true);
            }
        }

        
    }

    public void Awake(){
        Application.targetFrameRate = 60;
    }

    void SetTrackableHit(TrackableHit trackableHit){
        modelController.SetTrackableHit(trackableHit);

    }
}
