﻿using UnityEngine;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public enum Stratagy
    {
        Pass,
        Shoot
    }

    // Use this for initialization
    private readonly Random rnd = new Random();

    private GameObject _ball;
    private Vector3 _tempDestination;
    public float AngleCosMax = 0.8f;
    public Stratagy AI_Stratagy = Stratagy.Pass;
    public float BallDistance = 1.5f;
    public float PassForce = 10.0f;
    public float ShootForce = 300.0f;
    public float ShootDistance = 20.0f;
    public GameManager.Side side = GameManager.Side.Computer;
    public float Speed = 3.0f;
    public Vector3 TargetGoal = 55 * Vector3.back;
    public float up = 1.0f;
    public float X_MAX;
    public float distanceToGoal;

    private void Start()
    {
        _ball = GameObject.FindGameObjectWithTag("Football");
        _tempDestination = GetDestination();
    }

    private Vector3 GetDestination()
    {
        var result = Vector3.zero;
        result[0] = rnd.Next((int) -X_MAX, (int) X_MAX);
        result[2] = rnd.Next(-10000, -55);
        return result;
    }

    private float DistanceToBall()
    {
        return (_ball.transform.position - gameObject.transform.position).magnitude;
    }

    private void MoveTo(Vector3 pos)
    {
        var transformForward = pos - gameObject.transform.position;
        transformForward.y = 0;
        gameObject.transform.forward = transformForward;
        var rb = gameObject.GetComponent<Rigidbody>();
        rb.MovePosition(gameObject.transform.position + transformForward.normalized * Speed * Time.deltaTime);
    }

    private bool NearTargetGoal()
    {
        
        return distanceToGoal < ShootDistance;
    }
    // Update is called once per frame
    private void Update()
    {
        distanceToGoal = (TargetGoal - gameObject.transform.position).magnitude;
        if (DistanceToBall() > BallDistance + 0.2)
        {
            MoveBehind(_tempDestination);
        }
        else
        {
            if (NearTargetGoal()||Vector3.Dot(gameObject.transform.forward.normalized, (TargetGoal - gameObject.transform.position).normalized) > AngleCosMax)
            {
                MoveTo(_ball.transform.position);
            }
            else
            {
                var t = Vector3.Dot(gameObject.transform.forward, Vector3.right)>0?1:-1;
                var rb = gameObject.GetComponent<Rigidbody>();
                gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, 180 * Time.deltaTime*t);
                _ball.transform.RotateAround(gameObject.transform.position, Vector3.up, 180 * Time.deltaTime*t);
            }
        }
    }

    private void MoveBehind(Vector3 tempDestination)
    {
        var d = _tempDestination - _ball.transform.position;
        d.y = 0;

        var des = _ball.transform.position - d.normalized * BallDistance;
        MoveTo(des);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            _ball = collision.gameObject;
            var distance = (_ball.transform.position - gameObject.transform.position).magnitude;
            
            var kickDirection = gameObject.transform.forward;

            kickDirection[1] = up;
            var kickForce = (float) rnd.NextDouble();
            Vector3 tf;
            if (distanceToGoal<ShootDistance)
            {
                kickDirection = (TargetGoal - gameObject.transform.position).normalized;
                kickDirection[1] = up;
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
            }
            else if (AI_Stratagy==Stratagy.Shoot)
            {
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
                
            }
            else
            {
                kickDirection[1] = 0.5f;
                tf = kickDirection * 1.0f * PassForce;
            }

            _ball.GetComponent<Rigidbody>().AddForce(tf);
            GameManager.gm.LastBallTouch = side;
            _tempDestination = GetDestination();
        }
    }
}