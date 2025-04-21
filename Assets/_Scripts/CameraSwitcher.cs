using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera overWorldCamera;
    public CinemachineCamera battleCamera;
    public CinemachineCamera townCamera;
    
    // Current active camera
    private CinemachineCamera activeCamera;
    
    void Start()
    {
        // Set overworld camera as default
        SwitchToCamera(overWorldCamera);
    }
    
    public void SwitchToOverworld()
    {
        SwitchToCamera(overWorldCamera);
    }
    
    public void SwitchToBattle()
    {
        SwitchToCamera(battleCamera);
    }
    
    public void SwitchToTown()
    {
        SwitchToCamera(townCamera);
    }
    
    private void SwitchToCamera(CinemachineCamera newCamera)
    {
        // Deactivate current camera
        if (activeCamera != null)
            activeCamera.Priority = 0;
            
        // Activate new camera
        newCamera.Priority = 10;
        activeCamera = newCamera;
    }
}