using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "00b7dfed21f13c825fcd20b169592d69b960bd3f")]
public class camp : Component
{
	public float radius = 2; // ������ ������
	public float health = 1000; // �������� ������

	public Node target; // ���� ��� �������
	
	private WorldBoundSphere bs;

    void Init()
	{
		Console.Onscreen = true;
		Visualizer.Enabled = true;

        bs = new WorldBoundSphere(node.WorldPosition, radius);
    }
	
	void Update()
	{
		// ���� ������ � ������� ������ (�������� �� bullet)
		List<Node> nodes = new List<Node>();
		World.GetIntersection(bs, nodes);
		foreach (Node n in nodes)
		{
			mosquitto m = GetComponentInParent<mosquitto>(n);
			if (m)
			{
				// ���� ����� ������� � ������ - ������� ��� � ������� ���� �����, �.�. �� �� �������� �� �������
				health -= m.damage;
				m.Kill();
				Console.OnscreenMessageLine("health {0}", health); // ������� ������� ���� ��������
			}
		}

		// ���� ������������� ��� �������

		// ���� �������� ���������
		// if (health <= 0) {
        //     Console.OnscreenMessageLine("YOU LOOOOOOOOOOSER");
        //     Game.Scale = 0.0f; // ������������� ����
		// }
		
	}
}