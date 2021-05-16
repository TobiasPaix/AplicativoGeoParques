using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OverlayGeoParques : MonoBehaviour
{
    /// <summary>
    /// O Game Object que contém a janela com informações sobre Geoparque.
    /// </summary>
    [Tooltip(
        "O Game Object que contém a janela ou overlay com informações sobre o Geoparque ")]
    [FormerlySerializedAs("m_overlayInfoWindow")]
    [SerializeField]
    private GameObject _overlayInfoWindow = null;

    


    /// <summary>
    /// Unity's Start() method.
    /// </summary>
    public void Start()
    {
        CheckFieldsAreNotNull();
        _overlayInfoWindow.SetActive(false);
    }

    /// <summary>
    /// Função com retorno booleano para indicar se o overlay de informações está ativo ou não.
    /// </summary>
    public bool IsOverlayInfoActive(){
        return _overlayInfoWindow.activeSelf;
    }

    /// <summary>
    /// Callback executed when the open button has been clicked by the user.
    /// </summary>
    public void CanShowOverlay()
    {
        _overlayInfoWindow.SetActive(true);
    }

    /// <summary>
    /// Callback executed when the Got It button has been clicked by the user.
    /// </summary>
    public void CanCloseOverlay()
    {
        _overlayInfoWindow.SetActive(false);
    }



    /// <summary>
    /// Checks the required fields are not null, and logs a Warning otherwise.
    /// </summary>
    private void CheckFieldsAreNotNull()
    {
        if (_overlayInfoWindow == null)
        {
            Debug.LogError("MoreHelpWindow is null");
        }
    }
}
