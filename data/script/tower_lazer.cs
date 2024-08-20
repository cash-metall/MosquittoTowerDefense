using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unigine;

[Component(PropertyGuid = "3ec45a1f1a29b4adcbb6ec03cf38cbb7087b1ca1")]
public class tower : Component
{
	public float visibility_distance = 30; // ��������� ��������� ��� �����
	public float reload_time = 1; // ����� �����������

	public Node rotate_node; // ��� ���� ����� �������
	public Node muzzle; // �����. �� ���� ���� ����� �������� 

    public float rotation_speed = 60; // �������� �������� ����� �������/�������
	public float damage = 60.0f; // ����, ��������� ������


	private mosquitto target;
	private WorldBoundSphere bs;
	private float timer = 0.0f;

	void Init()
	{
		bs = new WorldBoundSphere(node.WorldPosition, visibility_distance);
	}
	
	void Update()
	{
		if (target && !target.node) // ���� ������� ���� ������ - �������� ��
		{
			target = null;
		}

		if (!target) // ���� ��� ���� - ���� ��
		{
			double distance = visibility_distance * visibility_distance;
            List<Node> nodes = new List<Node>();
            World.GetIntersection(bs, nodes); // ������ ���������� �� �����
            foreach (Node n in nodes) // ��������� ������ ����, �� ����� �� ���
            {
                mosquitto m = GetComponentInParent<mosquitto>(n);
                if (m)
                {
					// ����� ������� ���� ����������
					double l2 = MathLib.Distance2(n.WorldPosition, node.WorldPosition);
                    if (l2 < distance) 
                    {
						distance = l2;
						target = m;
                    }
                }
            }
        }

		// ���� ���� ����
		if (target) 
		{
			// ����� ����������� � ���������� �� ����
			vec3 dir_to_target = (vec3)(target.node.WorldPosition - rotate_node.WorldPosition);
			float distance = dir_to_target.Length;
			dir_to_target.Normalize();

			// ���� ����������� ����� � ����������� �� ���� ����� (��... �����) ��������!
			if (MathLib.Equals(dir_to_target, rotate_node.GetWorldDirection(MathLib.AXIS.Y), 0.01f))
			{
				// �����, ������ ��� ���������
				if (timer <= 0)
				{
					// ������� ����
					target.Damage(damage); 
					// ������ �������� ���
					Visualizer.RenderLine3D(rotate_node.WorldPosition, target.node.WorldPosition, vec4.BLUE, 0.1f);
					// ������� ������ �����������
					timer = reload_time;
				}
			}
			else
			{
				// ���� ���������� �� ���� �� ��������� � ����� ������������ - ������������ ������ �� ����
                var target_rotation = MathLib.RotationFromDir(dir_to_target, vec3.UP); // ������� �������� �� ������
				// rotateTowards ������������ �� �������� 1 �� �������� 2 �� ����
				// �������� 1 - ������� ��������
				// �������� 2 - ������� ��������
				// ���� = �������� �������� * �����
                var new_rotation = MathLib.RotateTowards(rotate_node.GetWorldRotation(), target_rotation, rotation_speed * Game.IFps);

				// ��������� ��������
                rotate_node.SetWorldRotation(new_rotation, true);
            }


        }

		if (timer > 0)
			timer -= Game.IFps;
	}
}