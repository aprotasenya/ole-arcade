using UnityEngine;

public class SpotGizmo : MonoBehaviour
{
    [SerializeField] Color gizmoColor = Color.yellow;
    [SerializeField, Range(0.01f, 1f)] float radius = 0.1f;
    [SerializeField] bool drawWireSphere = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        if (drawWireSphere) Gizmos.DrawWireSphere(transform.position, radius);
        if (!drawWireSphere) Gizmos.DrawSphere(transform.position, radius);
        
    }

}
