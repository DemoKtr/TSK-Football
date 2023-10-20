using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float mass;
    [SerializeField] private float friction;
    [SerializeField] private float radius;
    


    [SerializeField] private float initialForce = 0;
    private Vector3 distanceFromAxes = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 linearVelocity = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularVelocity = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 linearAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularAcceleration = new Vector3(0.0f,0.0f,0.0f);
    [SerializeField] private Vector3 forcePosition = new Vector3(0.0f,0.0f,0.0f); // tak naprawde raduius vectorowo
    private Vector3 force = new Vector3(0.0f,0.0f,0.0f); // odwrotnosc wwektora pozycji
    private Vector3 FrictionForce = new Vector3();
    
    
    //stale
    private const float forceTime = 0.01f;
    
        //zderzenie
            private const float damping = 15.176f; //c  N*s/m
            private const float stifftness = 36833f; // k N/m
        //wspolczynniki
            private const float kd = 0.00622f;
            private const float km = 0.25f;
        //grawitacja
            private const float g = 9.81f;

        // Start is called before the first frame update
    void Start()
    {
        calculate_Initial_Velocities();
        
    }

    // Update is called once per frame
    void Update()
    {
        //zrobić bool sprawdzający czy odbiliśmy się od ziemi jeśli taksprawdza czy nie ma kolizji z ziemią. collider może być z unity. jesli bool mowi ze jest na ziemi to robimy ruch ziemi ten fajny jesli nie to wiecej obliczen
        //
    }

    public void calculate_Initial_Velocities()
    {
        
        Vector3 forceDirection = -forcePosition;
        
        force.x = forceDirection.x / forceDirection.magnitude;
        force.y = forceDirection.y / forceDirection.magnitude;
        force.z = forceDirection.z / forceDirection.magnitude;
        force = force * initialForce;
        linearVelocity = (force * forceTime) / mass;
        //linearVelocity = (forceTime * initialForce)/mass;
        distanceFromAxes = new Vector3(
            Mathf.Sqrt(force.z * force.z + force.y * force.y),
            Mathf.Sqrt(force.z * force.z + force.x * force.x),
            Mathf.Sqrt(force.x * force.x + force.y * force.y));
        angularVelocity.x = ((5 * initialForce * distanceFromAxes.x) / ((radius * radius) * mass * 2));
    } 

    public Vector3 calculateColisionForce()
    {
        Vector3 collisionForce= new Vector3(0.0f,0.0f,0.0f);
        collisionForce.x = damping * linearVelocity.x + stifftness * 1 + kd * linearVelocity.x * linearVelocity.x;
        collisionForce.y = damping * linearVelocity.y + stifftness * 1 + kd * linearVelocity.x * Mathf.Abs(linearVelocity.x);
        collisionForce.z = damping * linearVelocity.z + stifftness * 1 + kd * linearVelocity.z * linearVelocity.z;
        return collisionForce;
    }

    public void calcualtelinearAcceleration()
    { 
        //To Do dodać na końce efekt coriolisa
        
        linearAcceleration.x = (km*linearVelocity.y*Mathf.Abs(linearVelocity.y)+km*linearVelocity.z*Mathf.Abs(linearVelocity.z)-kd*Mathf.Pow(linearVelocity.x,2))/mass;
        linearAcceleration.y = (-(kd*linearVelocity.y*Mathf.Abs(linearVelocity.y))+km*linearVelocity.z*Mathf.Abs(linearVelocity.z)-km*Mathf.Pow(linearVelocity.x,2)+mass*g)/mass;
        linearAcceleration.x = (-(km*Mathf.Pow(linearVelocity.x,2))+km*linearVelocity.y*Mathf.Abs(linearVelocity.y)+kd*Mathf.Pow(linearVelocity.z,2))/mass;
    }

    public void calculateFrictionForce()
    {
        //kierunek tej siły
        float FrictionForceMagnitude = mass * g * friction;
        FrictionForce = new Vector3((linearVelocity.x/linearVelocity.magnitude), (linearVelocity.y/linearVelocity.magnitude), (linearVelocity.z/linearVelocity.magnitude));
    }
}
