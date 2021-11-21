using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : Singleton<HUDController>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject carIconPrefab;
    [SerializeField] private GameObject peopleIconPrefab;
    private Dictionary<SixtCar, GameObject> carIcons = new Dictionary<SixtCar, GameObject>();
    private Dictionary<BookingData, GameObject> peopleIcons = new Dictionary<BookingData, GameObject>();
    
    public void UpdateCars(List<SixtCar> cars)
    {
        foreach (SixtCar car in cars)
        {
            if (!carIcons.ContainsKey(car))
            {
                carIcons.Add(car, Instantiate(carIconPrefab, canvas.transform));
            }
            PlaceGameObject(carIcons[car], SimulationVisualizer.Instance.GetNodeRepresentations()[car.CurrentNode].transform.position);
        }
    }

    public void EnablePerson(BookingData data)
    {
        if (!peopleIcons.ContainsKey(data))
        {
            peopleIcons.Add(data, Instantiate(peopleIconPrefab, canvas.transform));
        }
        
        peopleIcons[data].SetActive(true); 
        PlaceGameObject(peopleIcons[data], 
            SimulationVisualizer
                .Instance
                .GetNodeRepresentations()[NetworkController
                    .Instance.GetNodes()[data.StartNode]].transform.position);
    }

    public void DisablePerson(BookingData data)
    {
        if (peopleIcons.ContainsKey(data))
            peopleIcons[data].SetActive(false);
    }


    private void PlaceGameObject(GameObject icon, Vector3 realworldPos)
    {
        icon.GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(realworldPos);
    }
}
