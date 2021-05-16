using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class AugmentedImageController : MonoBehaviour
{
    /// <summary>
    /// A prefab for visualizing an AugmentedImage.
    /// </summary>
    public AugmentedImageVisualizerGeo AugmentedImageVisualizerGeoPrefab;

    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    public GameObject FitToScanOverlay;

    [SerializeField]
    private Button _openFitScreenButton;
    /// <summary>
    /// O estado da imagem FitToScreen, para que o aplicativo fique com menos coisas na tela.
    /// </summary>
    private bool _stateActiveFitScreen = false;
    
    /// <summary>
    /// O objeto com o script OverlayGeoparques. O script contém métodos que chamaremos nesse
    /// controlador quando acontecer certos eventos.
    /// </summary>
    public OverlayGeoParques OverlayInfo;

    /// <summary>
    /// The Game Object that contains the button to close the help window.
    /// </summary>
    [Tooltip("O botão que fecha a janela do overlay de informações.")]
    [SerializeField]
    private Button _gotItButton = null;

    /// <summary>
    /// Evento para indicar que é hora de mostrar o overlay.
    /// </summary>
    public UnityEvent _timeToShow = null;

    /// <summary>
    /// Variável para verificar se o botão GotIt do overlay foi clicado.
    /// </summary>
    private bool checkGotItButton = false;

    private Dictionary<int, AugmentedImageVisualizerGeo> _visualizers
        = new Dictionary<int, AugmentedImageVisualizerGeo>();

    private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();


    /// <summary>
    /// Unity's Start() method.
    /// </summary>
    public void Start()
    {
        _openFitScreenButton.onClick.AddListener(OnOpenButtonClicked);
        _gotItButton.onClick.AddListener(CanCloseOverlay);

        if(_timeToShow == null)
            _timeToShow = new UnityEvent();

        _timeToShow.AddListener(CanShowOverlay);
    }

    /// <summary>
    /// Unity's OnDestroy() method.
    /// </summary>
    public void OnDestroy()
    {
        _openFitScreenButton.onClick.RemoveListener(OnOpenButtonClicked);
        _gotItButton.onClick.RemoveListener(CanCloseOverlay);
        _timeToShow.RemoveListener(CanShowOverlay);  
    }

    /**
    *   A função é callback para o evento de identificar uma 
    *   imagem em 3D. Quando ARCore identificar a 
    *   imagem na base de dados, o overlay aparece.
    */
    public void CanShowOverlay(){
        OverlayInfo.CanShowOverlay();
    }

    public void CanCloseOverlay(){
        OverlayInfo.CanCloseOverlay();
        // Sleep();
        // PauseARCore();
    }

    /// <summary>
    /// Callback executed when the open button has been clicked by the user.
    /// </summary>
    private void OnOpenButtonClicked()
    {
        FitToScanOverlay.SetActive(!_stateActiveFitScreen);
        _stateActiveFitScreen = !_stateActiveFitScreen;
    }


    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update()
    {
        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(
            _tempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do
        // not previously have a visualizer. Remove visualizers for stopped images.
        foreach (var image in _tempAugmentedImages)
        {
            AugmentedImageVisualizerGeo visualizer = null;
            _visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
            if(OverlayInfo.IsOverlayInfoActive()){
                if(visualizer != null){
                    _visualizers.Remove(image.DatabaseIndex);
                    GameObject.Destroy(visualizer.gameObject);
                } 
                
                return;
            }
            
            if (image.TrackingState == TrackingState.Tracking && visualizer == null)
            {
                // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                Anchor anchor = image.CreateAnchor(image.CenterPose);
                visualizer = (AugmentedImageVisualizerGeo)Instantiate(
                    AugmentedImageVisualizerGeoPrefab, anchor.transform);
                visualizer.Image = image;
                _visualizers.Add(image.DatabaseIndex, visualizer);
                _timeToShow.Invoke();
                Destroy(anchor);
            }
            else if ((image.TrackingState == TrackingState.Stopped && visualizer != null) )
            {
                _visualizers.Remove(image.DatabaseIndex);
                GameObject.Destroy(visualizer.gameObject);
            }
            
        }
    }
}
