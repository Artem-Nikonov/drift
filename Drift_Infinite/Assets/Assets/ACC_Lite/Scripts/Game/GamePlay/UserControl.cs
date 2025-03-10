using SimpleInputNamespace;
using UnityEngine;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class UserControl : MonoBehaviour
{
	CarController ControlledCar;
	[SerializeField] private SteeringWheel steeringWheel;

	public float Horizontal { get; private set; }
	public float Vertical { get; private set; }
	public bool Brake { get; private set; }

	public static MobileControlUI CurrentUIControl { get; set; }

	private void Awake()
	{
		ControlledCar = GetComponent<CarController>();
		CurrentUIControl = FindObjectOfType<MobileControlUI>();
	}

	void Update()
	{
		// Standard input control (Keyboard or gamepad)
		Horizontal = Input.GetAxis("Horizontal");
		Vertical = Input.GetAxis("Vertical");
		Brake = Input.GetButton("Jump");

		Horizontal = steeringWheel.Value;

		if (steeringWheel.wheelBeingHeld)
		{
			Vertical = 1f;
		}
		else
		{
			Vertical = 0f;
		}

		// Apply control for controlled car
		ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
	}
}
