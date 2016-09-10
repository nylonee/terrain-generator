using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{

    public Color color;
    public float daySpeed = 1f;

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

    void Update()
    {
        this.transform.Rotate(new Vector3(-180, 0, 0) * Time.deltaTime * 0.001f * daySpeed);
    }
}
