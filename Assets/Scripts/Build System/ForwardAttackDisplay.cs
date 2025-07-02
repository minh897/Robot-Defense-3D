using UnityEngine;

public class ForwardAttackDisplay : MonoBehaviour
{
    [SerializeField] private float attackRange;
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private LineRenderer rightLine;


    void Awake()
    {
        leftLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;    
        rightLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;    
    }

    public void CreateLines(bool showLines, float newRange)
    {
        leftLine.enabled = showLines;
        rightLine.enabled = showLines;

        if (showLines == false)
            return;

        attackRange = newRange;
        UpdateLines();
    }

    public void UpdateLines()
    {
        DrawLine(leftLine);
        DrawLine(rightLine);
    }

    private void DrawLine(LineRenderer line)
    {
        Vector3 start = line.transform.position;

        // Extend the forward direction by desired distance 
        // and moves start forward by that much
        Vector3 end = start + transform.forward * attackRange;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
