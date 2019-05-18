using UnityEngine;
using System.Collections;

// This class simulates an anti-roll bar.
// Anti roll bars transfer suspension compressions forces from one wheel to another.
// This is used to minimize body roll in corners, and improve grip by balancing the wheel loads.
// Typical modern cars have one anti-roll bar per axle.
public class AntiRollBar : MonoBehaviour {

	// The two wheels connected by the anti-roll bar. These should be on the same axle.
	public WheelCollider wheel1;
	public WheelCollider wheel2;
	public Rigidbody rb;
	
	// Coefficient determining how much force is transfered by the bar.
	public float coefficient;
	
	void FixedUpdate () 
	{
		float force = (wheel1.suspensionSpring.targetPosition - wheel2.suspensionSpring.targetPosition) * coefficient;
		rb.AddForceAtPosition(wheel1.transform.up*-force, wheel1.transform.position);
		rb.AddForceAtPosition(wheel1.transform.up*force, wheel1.transform.position);
	}
}
