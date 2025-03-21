using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    private float smoothSpeed = 3f; // —корость интерпол€ции


    public CarTransformInfo? transformInfo = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transformInfo != null)
        {
            var pos = transformInfo.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, pos.z), smoothSpeed * Time.deltaTime);

            var rot = transformInfo.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(rot.x, rot.y, rot.z, rot.w), smoothSpeed * Time.deltaTime);
        }
    }
}