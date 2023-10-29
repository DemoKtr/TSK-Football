using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerControler : MonoBehaviour
{
    public GameObject marker; // Referencja do obiektu Plane (znacznika)
    private GameObject currentMarker; // Referencja do obecnie wyœwietlanego znacznika
    private float markerDistance = 0f; // Odleg³oœæ od kuli, na której ma byæ wyœwietlony znacznik
    public GameObject Sphere;
    bool CanBePlaced;
    RaycastHit hit;


    private void Start()
    {
        CanBePlaced = true;
        marker.SetActive(false); // Na pocz¹tku znacznik jest wy³¹czony
        marker.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(CanBePlaced)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.Equals(Sphere))
                {
                    if (currentMarker == null)
                    {
                        currentMarker = Instantiate(marker); // Tworzymy nowy znacznik
                    }

                    currentMarker.SetActive(true); // W³¹czamy znacznik
                    currentMarker.transform.position = hit.point + hit.normal * markerDistance; // Ustawiamy pozycjê znacznika
                    currentMarker.transform.up = hit.normal;
                }
                else
                {
                    if (currentMarker != null)
                    {
                        currentMarker.SetActive(false); // Jeœli nie trafiliœmy w kulê, wy³¹czamy znacznik
                    }
                }
            }
            else
            {
                if (currentMarker != null)
                {
                    currentMarker.SetActive(false); // Jeœli nie trafiliœmy na ¿aden obiekt, wy³¹czamy znacznik
                }
            }

        }
        
    }

    private void OnMouseDown()
    {
        currentMarker = Instantiate(marker);
        currentMarker.SetActive(true); // W³¹czamy znacznik
        currentMarker.transform.position = hit.point + hit.normal * markerDistance;
        currentMarker.transform.up = hit.normal;
        CanBePlaced = false;
    }
    

}
