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
    [SerializeField] private MarkerControler markercontroler;  // tak naprawde raduius vectorowo
    private Vector3 force = new Vector3(0.0f,0.0f,0.0f); // odwrotnosc wektora pozycji
    private Vector3 FrictionForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 MagnusForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 coriolisForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 ResultForce = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 collisionForce= new Vector3(0.0f,0.0f,0.0f);

    
    
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
    //boole 
    public bool onAir;
            public bool isKicked;
            public bool isCalculatedCollision;
            public bool isGoal;
            public bool isCollision;
            
           
            // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
       
        
        //zrobić bool sprawdzający czy odbiliśmy się od ziemi jeśli taksprawdza czy nie ma kolizji z ziemią. collider może być z unity. jesli bool mowi ze jest na ziemi to robimy ruch ziemi ten fajny jesli nie to wiecej obliczen
        //
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.ToString() == "pitch")
        {
            isCollision = true;
            isCalculatedCollision = false;
        }
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
        angularVelocity.x = ((5 * force.x * distanceFromAxes.x) / ((radius * radius) * mass * 2));
        angularVelocity.x = ((5 * force.y * distanceFromAxes.y) / ((radius * radius) * mass * 2));
        angularVelocity.x = ((5 * force.z * distanceFromAxes.z) / ((radius * radius) * mass * 2));
    } 

    public void calculateColisionForce()
    {
        //x y z to wektor odkształcenia
        
        
        collisionForce.x = damping * linearVelocity.x + stifftness * deviation.x + kd * linearVelocity.x * linearVelocity.x;
        collisionForce.y = damping * linearVelocity.y + stifftness * deviation.y + kd * linearVelocity.x * Mathf.Abs(linearVelocity.x);
        collisionForce.z = damping * linearVelocity.z + stifftness * deviation.z + kd * linearVelocity.z * linearVelocity.z;
       
    }
    private IEnumerator MyCoroutine(float waitTime)
    {
        while (true)
        {
            if (isCollision)
            {
                if (!isCalculatedCollision)
                {
                    calculateColisionForce();
                    ResultForce = collisionForce;
                    isCalculatedCollision = true;
                }
                isCollision = false;
            }
            else
            {
                calculateResultForce();
                
            }
            calculateAcceleration();
            
            calculateVelocities(waitTime);
            calculatePositions(waitTime);
            
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void calculateAcceleration()
    {
        calculateAngularAcceleration();
        calcualtelinearAcceleration();
    }
    public void calculateVelocities(float dt)
    {
        calculateAngularVelocity(dt);
        calculateLinearVelocity(dt);
    }

    public void calculatePositions(float dt)
    {
        calculatePosition(dt);
       // calculateRotation(dt);
    }
    public void calcualtelinearAcceleration()
    { 
        // Ten co to robil ma tu zajrzec
        linearAcceleration = ResultForce/mass;
    }
    public void calcualteCoriolisFoce()
    {
        coriolisForce = 2 * mass * Vector3.Cross(EarthAngularVelocity , linearVelocity);
    }

    public void calculateResultForce()
    {
        calculateMagnusForce();
        calcualteCoriolisFoce();
        calculateFrictionForce();
        ResultForce = MagnusForce + coriolisForce + FrictionForce;
    }
    public void calculateMagnusForce()
    {
        MagnusForce = new Vector3(
            km * linearVelocity.y * Mathf.Abs(linearVelocity.y) + km * linearVelocity.z * Mathf.Abs(linearVelocity.z) -
            kd * Mathf.Pow(linearVelocity.x, 2),
            -(kd * linearVelocity.y * Mathf.Abs(linearVelocity.y)) +
            km * linearVelocity.z * Mathf.Abs(linearVelocity.z) - km * Mathf.Pow(linearVelocity.x, 2) + mass * g,
            -(km * Mathf.Pow(linearVelocity.x, 2)) + km * linearVelocity.y * Mathf.Abs(linearVelocity.y) +
            kd * Mathf.Pow(linearVelocity.z, 2)
        );
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
   /*
    public void calculateRotation(float dt)
    {
        this.transform.Rotate(
            angularVelocity.x * dt + ((angularAcceleration.x*dt*dt)/2), 
            angularVelocity.y * dt + ((angularAcceleration.y*dt*dt)/2), 
            angularVelocity.z* dt + ((angularAcceleration.z*dt*dt)/2),
            Space.Self);
    }*/
    public void calculateLinearVelocity(float dt)
    {
        linearVelocity += linearAcceleration * dt;
    }
    public void calculateAngularVelocity(float dt)
    {
        angularVelocity += angularAcceleration * dt;
    }
    public void calculateAngularAcceleration()
    {
        if (FrictionForce != Vector3.zero)
        {
            Vector3 helperRadiusVector = this.GetComponent<SphereCollider>().center - new Vector3(0, -radius, 0);
            angularAcceleration = (Vector3.Cross(FrictionForce, helperRadiusVector))/((5/2)*mass*radius*radius);
            // M/I I =2/5 mr^2
        }
        else
        {
            angularAcceleration = Vector3.zero;
        }
    }
    public void Launch()
    {
        mass = MassSlider.value;
        initialForce = ForceSlider.value; //zaufaj ~Przemysław
        forcePosition = markercontroler.InitialPosition;
        Debug.Log(forcePosition);
        Debug.Log("kurwa");
        calculate_Initial_Velocities();
        StartCoroutine(MyCoroutine(0.01f));
    }

}
