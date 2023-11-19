using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MarkerControler : MonoBehaviour
{
    public GameObject marker; // Referencja do obiektu Plane (znacznika)
    public GameObject rayPlane;
    private GameObject currentMarker; // Referencja do obecnie wy�wietlanego znacznika
    private float markerDistance = 0f; // Odleg�o�� od kuli, na kt�rej ma by� wy�wietlony znacznik
    public GameObject Sphere;
    bool CanBePlaced;
    bool CanPlaceVector;
    RaycastHit hit;
    [SerializeField] LaunchButtonController launchButtonController;
    public Vector3 InitialPosition;
    private LineRenderer lineRenderer;
    private Vector3 distance;
    public Vector3 ForceVector;

    private void Start()
    {
        CanBePlaced = true;
        CanPlaceVector = false;
        marker.SetActive(false); // Na pocz�tku znacznik jest wy��czony
        marker.layer = LayerMask.NameToLayer("Ignore Raycast");
        // Dodane utworzenie i konfiguracja LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.01f; // Dostosuj szerokość linii według potrzeb
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(CanPlaceVector)
        {
            rayPlane.transform.position = currentMarker.transform.position + distance * 0.15f;
            //rayPlane.transform.up = currentMarker.transform.up;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == rayPlane)
            {
                Vector3 mouseToMarker = currentMarker.transform.position - hit.point;

                lineRenderer.SetPosition(0, hit.point);
                lineRenderer.SetPosition(1, hit.point + mouseToMarker);
                lineRenderer.enabled = true;

                if (Input.GetMouseButtonDown(0))
                {
                    MakeLine(mouseToMarker);
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        if (CanBePlaced)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.Equals(Sphere))
                {
                    if (currentMarker == null)
                    {
                        currentMarker = Instantiate(marker); // Tworzymy nowy znacznik
                    }

                    currentMarker.SetActive(true); // W��czamy znacznik
                    currentMarker.transform.up = hit.normal;
                    currentMarker.transform.position = hit.point + hit.normal * markerDistance; // Ustawiamy pozycj� znacznika


                    if (Input.GetMouseButtonDown(0))
                    {
                        distance = hit.normal;
                        MakeMarker();
                    }
                }
                else
                {
                    if (currentMarker != null)
                    {
                        currentMarker.SetActive(false); // Je�li nie trafili�my w kul�, wy��czamy znacznik
                    }
                }
            }
            else
            {
                if (currentMarker != null)
                {
                    currentMarker.SetActive(false); // Je�li nie trafili�my na �aden obiekt, wy��czamy znacznik
                }
            }

        }
        
    }

    private void MakeMarker()
    {
        InitialPosition = currentMarker.transform.position;
        launchButtonController.HitPointNotNull = true;
        CanBePlaced = false;
        CanPlaceVector = true;

    }
    private void MakeLine(Vector3 force)
    {
        ForceVector = force;
        lineRenderer.SetPosition(0, hit.point);
        lineRenderer.SetPosition(1, hit.point + force);
        lineRenderer.enabled = true;
        CanPlaceVector = false;

    }

    public void ResetValues()
    {
        CanBePlaced = true;
        marker.SetActive(false);
        marker.layer = LayerMask.NameToLayer("Ignore Raycast");
        launchButtonController.HitPointNotNull = false;
        rayPlane.transform.position = new Vector3(-100, -100, 0);
        lineRenderer.enabled = false;
        CanBePlaced = true;
    }

}
