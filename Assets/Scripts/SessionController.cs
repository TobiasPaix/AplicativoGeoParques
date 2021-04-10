//-----------------------------------------------------------------------
// <copyright file="GeoParquesController.cs" company="Google LLC">
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
using GoogleARCore.Examples.Common; // Esse aqui é para o DepthMenu, e talvez para 
                                    //  InstantPlacementMenu também.

#if UNITY_EDITOR
// Habilita a propagação do toque no celular para o computador enquanto em Instant Preview.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class SessionController : MonoBehaviour
{
    /// <summary>
    /// Verdadeira se o aplicativo está no processo de sair devido a um erro ao conectar
    /// ARCore, caso contrário é falso.
    /// </summary>
    private bool _isQuitting = false;


    // Update is called once per frame
    void Update()
    {
        UpdateApplicationLifecycle();
    }

    public void Awake(){
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// Está relacionado ao ciclo de vida do aplicativo e lida com comandos para
    /// sair do aplicativo, manter a tela ligada enquanto estiver rastreando e 
    /// com mensagens de erro.
    /// </summary>
    private void UpdateApplicationLifecycle(){
        // Sair do aplicativo quando "Voltar for pressionado
        if(Input.GetKey(KeyCode.Escape)){
            Application.Quit();
        }

        // Só permitir apagar a tela quando não estiver rastreando
        if(Session.Status != SessionStatus.Tracking){
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else{
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if(_isQuitting){
            return;
        }

        // Sair se ARCore não conseguiu se conectar e dar tempo pro Unity mostrar a mensagem torrada.
        if(Session.Status == SessionStatus.ErrorPermissionNotGranted){
            ShowAndroidToastMessage("Precisamos da permissão do uso da câmera para o aplicativo funcionar.");
            _isQuitting = true;
            Invoke("DoQuit", 0.5f); // Invoca o DoQuit em 0.5 segundos.
        }
        else if(Session.Status.IsError()){
            ShowAndroidToastMessage(
                "ARCore encontrou um problema ao se conectar. Por favor, inicie o aplicativo novamente."
            );
            _isQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }


    /// <summary>
    /// Realmente sair do aplicativo.
    /// </summary>
    public void DoQuit(){
        Application.Quit();
    }


    /// <summary>
    /// Mostra uma mensagem de erro torrada do Android.
    /// </summary>
    private void ShowAndroidToastMessage(string message){
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = 
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        if (unityActivity != null){ // Se há algum tipo de atividade, então
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = 
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
    
}
