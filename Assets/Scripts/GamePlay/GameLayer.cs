using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] public LayerMask solidObjectsLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public LayerMask grassLayer;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask fovLayer;
    [SerializeField] public LayerMask portalLayer;
    [SerializeField] public LayerMask triggersLayer;
    [SerializeField] public LayerMask ledgesLayer;
    [SerializeField] public LayerMask waterLayer;

    public static GameLayer i { get; set; }
    public void Awake()
    {
        i = this;
    }

    public LayerMask SolidObjectsLayer { get { return solidObjectsLayer; } }
    public LayerMask InteractableLayersLayer { get { return interactableLayer; } }
    public LayerMask GrassLayersLayer { get { return grassLayer; } }
    public LayerMask PlayerLayer { get { return playerLayer; } }
    public LayerMask FovLayer { get { return fovLayer; } }
    public LayerMask PortalLayer { get { return portalLayer; } }
    public LayerMask LedgesLayer { get { return ledgesLayer; } }
    public LayerMask WaterLayer { get { return waterLayer; } }

    public LayerMask TriggerableLayer { get => grassLayer | fovLayer | portalLayer | triggersLayer | waterLayer; }
}
