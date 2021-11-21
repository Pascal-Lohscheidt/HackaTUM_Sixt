using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Singleton<CarController>
{
   [SerializeField] private int amountOfCars;
   [SerializeField] private float executionIntervalLength;
   public event Action ReceivedNewBooking;
   public event Action ExecuteCarLogic;

   private List<SixtCar> cars = new List<SixtCar>();
   private float executionTimeStamp;
   
   public void GenerateCars()
   {
      NetworkController networkController = NetworkController.Instance;
      for (int i = 0; i < amountOfCars; i++)
      {
         cars.Add(new SixtCar(
            i, networkController
               .GetNodes()[UnityEngine.Random.Range(0, networkController.GetNodes().Length - 1)]
            ));
      }
   }

   public void Update()
   {
      if (Time.time - executionTimeStamp > executionIntervalLength && SimulationController.timeScale > 0)
      {
         executionTimeStamp = Time.time;
         ExecuteCarLogic?.Invoke();
      }
   }

   public void CheckForBookings()
   {
      ReceivedNewBooking?.Invoke();
   }
   
}
