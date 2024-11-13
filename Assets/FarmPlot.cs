using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private int radius;
    [SerializeField] private float gridStep;
    public List<Vector2> plotPoints;

    void Start() {
        plotPoints = GetPoints();
    }
    
    private List<Vector2> GetPoints() {
        List<Vector2> points = new List<Vector2>();

        for (float x=transform.position.x-(radius*gridStep); x<=transform.position.x+(radius*gridStep); x+=gridStep) {
            for (float y=transform.position.y-(radius*gridStep); y<=transform.position.y+(radius*gridStep); y+=gridStep) {
                points.Add(new Vector2(x, y));
            }
        }

        return points;
    }
}