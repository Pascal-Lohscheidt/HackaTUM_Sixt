using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookingController : Singleton<BookingController>
{
    [SerializeField] private float timeBetweenBooking;
    
    private float timeStamp = 0;
    private Queue<BookingData> bookings = new Queue<BookingData>();
    private List<BookingData> completedBookings = new List<BookingData>();

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeStamp > timeBetweenBooking && SimulationController.timeScale > 0)
        {
            timeStamp = Time.time;
            MakeBooking();
        }
    }

    public BookingData GetBookingEntry()
    {
        if (bookings.Count > 0)
        {
            return bookings.Dequeue();
        }

        return null;
    }

    public void CompleteBooking(BookingData data)
    {
        Debug.Log("Booking Execution Completed");
        completedBookings.Add(data);
    }
    
    public void MakeBooking()
    {
        Debug.Log("Booking Created");
        NetworkController networkController = NetworkController.Instance;
        int start = Random.Range(0, networkController.GetNodes().Length - 1);
        int end = Random.Range(0, networkController.GetNodes().Length - 1);

        if (start == end) start = (start + end) % networkController.GetNodes().Length - 1;
        BookingData data = new BookingData(start, end, Time.time);
        bookings.Enqueue(data);
        CarController.Instance.CheckForBookings();
    }
}
