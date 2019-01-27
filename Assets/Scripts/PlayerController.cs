﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 25;
	public Transform anchorTransform;

	private Vector3 _rotationAxis;
	private Transform _playerTransform;
    private bool _isAnchored = true;
    private float _rotationSpeed;

    //Collisions


    // Use this for initialization
    private void Start()
    {
        _playerTransform = gameObject.transform;
        UpdateRotation();
    }
	
	// Update is called once per frame
	private void Update () {
        if (_isAnchored)
        {
	        if (Input.GetButtonDown("Jump"))
	        {
		        _isAnchored = false;
	        }
	        _playerTransform.RotateAround(anchorTransform.position, _rotationAxis, _rotationSpeed * Time.deltaTime);
        }
        else
        {
	        TryToAnchor();
            _playerTransform.Translate(speed * Vector3.up * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        GameManager.Instance.PlayerCrash();
    }

    private void TryToAnchor()
	{
		foreach (var planet in GameManager.Instance.planets)
		{
			if (planet.transform == anchorTransform) continue;
			var radius = _playerTransform.position - planet.gameObject.transform.position;
			if (radius.magnitude < planet.GravityRadius && Vector3.Angle(_playerTransform.up, radius) - 90 < 1.5)
			{
				anchorTransform = planet.transform;
				UpdateRotation();
				_isAnchored = true;
				return;
			}
		}
	}

	private void UpdateRotation()
	{
		//update rotation axis
		var direction = _playerTransform.up;
		var radius = _playerTransform.position - anchorTransform.position;
		_rotationAxis = Vector3.Cross(radius, direction).y < 0 ? Vector3.down : Vector3.up;
		
		//update rotation speed (deg/sec)
		var perimeter = 2 * Mathf.PI * radius.magnitude;
		_rotationSpeed = speed * 360 / perimeter;
		
		//update ship angle
		var modifier = _rotationAxis.y < 0 ? -90 : 90; 
		var angle = Vector3.SignedAngle(direction, radius, Vector3.up) + modifier;
		_playerTransform.Rotate(Vector3.up, angle, Space.World);
	}
}
