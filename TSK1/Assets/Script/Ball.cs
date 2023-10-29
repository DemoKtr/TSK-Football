using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    [SerializeField] Slider ForceSlider;        //zaufaj, tak trzeba bo unity nas nie lubi ~ Przemysław
    [SerializeField] Slider MassSlider;

    [SerializeField] private float mass;
    [SerializeField] private float friction;
    [SerializeField] public float radius;
    private Vector3 EarthAngularVelocity=new Vector3(0.0f,0.0000727f,0f);
    


    [SerializeField] private float initialForce = 0;
    private Vector3 distanceFromAxes = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 linearVelocity = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularVelocity = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 linearAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 angularAcceleration = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 deviation = new Vector3(1f,1f,1f);
    [SerializeField] private Vector3 forcePosition = new Vector3(0.0f,0.0f,0.0f); // tak naprawde raduius vectorowo
    private Vector3 force = new Vector3(0.0f,0.0f,0.0f); // odwrotnosc wwektora pozycji
    private Vector3 FrictionForce = new Vector3();
    private Vector3 FrictionForceMomentum = new Vector3();
    private Vector3 coriolisForce = new Vector3();

    private Transform thisTransform;
    
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
        this.thisTransform = GetComponent<Transform>();
        calculate_Initial_Velocities();
        
    }

    // Update is called once per frame
    void Update()
    {
        //zrobić bool sprawdzający czy odbiliśmy się od ziemi jeśli taksprawdza czy nie ma kolizji z ziemią. collider może być z unity. jesli bool mowi ze jest na ziemi to robimy ruch ziemi ten fajny jesli nie to wiecej obliczen
        //
    }

    public void OnCollisionEnter(Collision other)
    {
        calculateColisionForce();
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
        //x y z to wektor odkształcenia
        
        Vector3 collisionForce= new Vector3(0.0f,0.0f,0.0f);
        collisionForce.x = damping * linearVelocity.x + stifftness * deviation.x + kd * linearVelocity.x * linearVelocity.x;
        collisionForce.y = damping * linearVelocity.y + stifftness * deviation.y + kd * linearVelocity.x * Mathf.Abs(linearVelocity.x);
        collisionForce.z = damping * linearVelocity.z + stifftness * deviation.z + kd * linearVelocity.z * linearVelocity.z;
        return collisionForce;
    }

    public void calcualtelinearAcceleration()
    { 
        //To Do dodać na końce efekt coriolisa
        
        linearAcceleration.x = (km*linearVelocity.y*Mathf.Abs(linearVelocity.y)+km*linearVelocity.z*Mathf.Abs(linearVelocity.z)-kd*Mathf.Pow(linearVelocity.x,2)+coriolisForce.x)/mass;
        linearAcceleration.y = (-(kd*linearVelocity.y*Mathf.Abs(linearVelocity.y))+km*linearVelocity.z*Mathf.Abs(linearVelocity.z)-km*Mathf.Pow(linearVelocity.x,2)+mass*g+coriolisForce.y)/mass;
        linearAcceleration.x = (-(km*Mathf.Pow(linearVelocity.x,2))+km*linearVelocity.y*Mathf.Abs(linearVelocity.y)+kd*Mathf.Pow(linearVelocity.z,2)+coriolisForce.z)/mass;
    }
    public void calcualteCoriolisFoce()
    {
        coriolisForce = 2 * mass * Vector3.Cross(EarthAngularVelocity , linearVelocity);
    }
    
    public void calculateFrictionForce()
    {
        //kierunek tej siły
        float FrictionForceMagnitude = mass * g * friction;
        FrictionForce = new Vector3((linearVelocity.x/linearVelocity.magnitude), (linearVelocity.y/linearVelocity.magnitude), (linearVelocity.z/linearVelocity.magnitude));
        FrictionForce = FrictionForce * FrictionForceMagnitude;
        
    }

    public void calculatePosition(float dt)
    {
        this.transform.position = linearVelocity * dt + ((linearAcceleration * dt * dt) / 2);

    }
    public void calculateRotation(float dt)
    {
        this.transform.Rotate(
            angularVelocity.x * dt + ((angularAcceleration.x*dt*dt)/2), 
            angularVelocity.y * dt + ((angularAcceleration.y*dt*dt)/2), 
            angularVelocity.z* dt + ((angularAcceleration.z*dt*dt)/2),
            Space.Self);
    }
    public void calculateLinearVelocity(float dt)
    {
        linearVelocity += linearAcceleration * dt;
    }
    public void calculateAngularVelocity(float dt)
    {
        angularVelocity += angularAcceleration * dt;
    }
    public void Launch()
    {
        mass = MassSlider.value;
        initialForce = ForceSlider.value; //zaufaj ~Przemysław
    }

}
