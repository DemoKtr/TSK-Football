using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerControler : MonoBehaviour
{
    public GameObject marker; // Referencja do obiektu Plane (znacznika)
    private GameObject currentMarker; // Referencja do obecnie wy�wietlanego znacznika
    private float markerDistance = 0f; // Odleg�o�� od kuli, na kt�rej ma by� wy�wietlony znacznik
    public GameObject Sphere;
    bool CanBePlaced;
    RaycastHit hit;


    private void Start()
    {
        CanBePlaced = true;
        marker.SetActive(false); // Na pocz�tku znacznik jest wy��czony
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

                    currentMarker.SetActive(true); // W��czamy znacznik
                    currentMarker.transform.position = hit.point + hit.normal * markerDistance; // Ustawiamy pozycj� znacznika
                    currentMarker.transform.up = hit.normal;
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

    private void OnMouseDown()
    {
        currentMarker = Instantiate(marker);
        currentMarker.SetActive(true); // W��czamy znacznik
        currentMarker.transform.position = hit.point + hit.normal * markerDistance;
        currentMarker.transform.up = hit.normal;
        CanBePlaced = false;
    }
    

}
