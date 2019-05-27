using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Breach.Placeable;
using Breach.Placeable.Pathfinding;
using Breach.Manager;

namespace Breach.Controler
{
    public class CameraRaycaster : SingletionBase<CameraRaycaster>
    {

        const float maxRaycastDepth = 100f;

        Rect screenRectOnConstruction = new Rect(0, 0, Screen.width, Screen.height);

        public delegate void OnMouseOverWalkableTiles(Waypoint waypoint);
        public event OnMouseOverWalkableTiles onMouseOverWalkableTiles;

        public delegate void OnMouseOverSelectable(GameObject selectableGO);
        public event OnMouseOverSelectable onMouseOverSelectable;

        public delegate void OnMouseOverWorld();
        public event OnMouseOverWorld onMouseOverWorld;

        void Update()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // dont raycast if over UI element
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if (screenRectOnConstruction.Contains(Input.mousePosition))
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                // specify layer priorities below, order matters!
                if (RaycastForSelectable(hits)) { return; }
                if (RaycastForWalkableTile(hits)) { return; }
                if (RaycastForWorld(hits)) { return; }
            }
        }

        private bool RaycastForSelectable(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    var gameObjectHit = hit.collider.gameObject;
                    var selectableHit = gameObjectHit.GetComponent<ISelectable>();
                    if (selectableHit != null)
                    {
                        if (selectableHit.GetSelectability() == true)
                        {
                            if (onMouseOverSelectable != null)
                            {
                                onMouseOverSelectable(selectableHit.GetGameObject());
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool RaycastForWalkableTile(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    var gameObjectHit = hit.collider.gameObject;
                    var WaypointHit = gameObjectHit.GetComponent<Waypoint>();
                    if (WaypointHit)
                    {
                        if (onMouseOverWalkableTiles != null)
                        {
                            onMouseOverWalkableTiles(WaypointHit);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool RaycastForWorld(RaycastHit2D[] hits)
        {
            if (onMouseOverWorld != null)
            {
                onMouseOverWorld();
            }
            return true;
        }
    }
}