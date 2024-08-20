using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "e44c97c7e488c3d58346dc2b5b3e830fa189eda8")]
public class bullet : Component
{
	public float speed = 6.0f; // �������� ����
	public float damage = 30.0f; // ���� ����
	public float distance = 50f; // ������������ ��������� ������ ����
	public float radius = 0.1f; // ������ ����

	public AssetLink splash; // ������ �� ������ ��� ���������

	private dvec3 init_pos; 
	void Init()
	{
		// ��������� ������� � ������� ���� ���������, ����� ������� ������� ��� ���������
		init_pos = node.WorldPosition;
	}

	void Update()
	{
		// ���� ����� ������
		node.Translate(0, Game.IFps * speed, 0);

		// ���� ��������� �� ����� �� ��� ������.
		// ����: ���������� �� ����� �� ������ current_pos - prev_pos, ������ ��� ����� ������� ���� ����� ���������� ������ ������

        List<Node> nodes = new List<Node>();
        // �������� ��� ���� � ������� ����
		World.GetIntersection(new WorldBoundSphere(node.WorldPosition, radius), nodes);
        // ���� ����� ��� ������
		foreach (Node n in nodes)
        {
            mosquitto m = GetComponentInParent<mosquitto>(n);
            if (m)
            {
				// ���� �����, ��������� ����� ������
				m.Damage(damage);
				// ������� ����
				node.DeleteLater();
				// ������� ������ ���������
				Node s = World.LoadNode(splash.Path);
				s.WorldTransform = node.WorldTransform;
            }
        }


		// ���� ���� ������� ������ ������������ ��������� - ������� ��
        if (MathLib.Distance2(init_pos, node.WorldPosition) > distance * distance)
			node.DeleteLater(); 
    }
}