using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableMesh : MonoBehaviour
{
    [SerializeField] private MeshFilter m_MeshFilter;

    private Mesh m_Mesh;
    private Vector3[] m_Verts;
    private Color32[] m_VertColors;

    private void Awake()
    {
        m_Mesh = m_MeshFilter.mesh;
        m_Verts = m_Mesh.vertices;
        m_VertColors = new Color32[m_Verts.Length];
        for (int i = 0; i < m_VertColors.Length; i++)
        {
            m_VertColors[i] = Color.white;
        }
    }

    public void ApplyPaint(Vector3 position, float innerRadius, float outerRadius, Color color)
    {
        Vector3 center = transform.InverseTransformPoint(position);
        float outerR = transform.InverseTransformVector(outerRadius * Vector3.right).magnitude;
        float innerR = innerRadius * outerR / outerRadius;
        float innerRsqr = innerR * innerR;
        float outerRsqr = outerR * outerR;
        float tFactor = 1f / (outerR - innerR);

        for (int i = 0; i < m_Verts.Length; i++)
        {
            Vector3 delta = m_Verts[i] - center;
            float dsqr = delta.sqrMagnitude;
            if (dsqr > outerRsqr) continue;
            int a = m_VertColors[i].a;
            m_VertColors[i] = color;
            if (dsqr < innerRsqr) m_VertColors[i].a = 255;
            else
            {
                float d = Mathf.Sqrt(dsqr);
                byte blobA = (byte)(255 - 255 * (d - innerR) * tFactor);
                if (blobA >= a) m_VertColors[i].a = blobA;
            }
        }
        m_Mesh.colors32 = m_VertColors;
    }
}
