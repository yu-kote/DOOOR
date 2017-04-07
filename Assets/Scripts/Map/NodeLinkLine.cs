using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLinkLine : MonoBehaviour
{
    private LineRenderer _line;

    private Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }
    
    void Awake()
    {
        CreateLine();
    }

    public void SetLinkNode(Node node)
    {
        _targetNode = node;
        Link();
    }

    private void CreateLine()
    {
        _line = gameObject.AddComponent<LineRenderer>();
        _line.material = new Material(Shader.Find("Particles/Additive"));

        var c1 = Color.yellow;
        var c2 = Color.red;

        _line.startColor = c1;
        _line.endColor = c2;
        _line.startWidth = 0.1f;
        _line.endWidth = 0.1f;
        _line.numPositions = 2;
    }

    public void Link()
    {
        var start_position = gameObject.transform.parent.position;
        var end_position = _targetNode.transform.position;
        _line.SetPosition(0, start_position);
        _line.SetPosition(1, end_position);
    }

    private void OnDestroy()
    {
        Destroy(_line);
    }

    void Update()
    {

    }
}
