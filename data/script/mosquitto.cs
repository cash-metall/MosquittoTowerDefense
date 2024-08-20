using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c5f6f4ec4d2e7a1283220678a6aa9c6448cff312")]
public class mosquitto : Component
{
	public ObjectBillboards health_bar; // ��������� �� ���������-������� ��������
	public float rot_speed = 30.0f; // �������� �������� � �������� � �������
	public float move_speed = 1.0f; // �������� �������� � ������ � �������
	public float damage = 100.0f; // ����, ������� ������� �����, ����������� �� ������
	public float max_health = 100.0f; // ��������� �������� ������

	private Node target; // ������� ����
	private float health_bar_width; // �������� ������
	private float health; // ������� ��������

	// ���� ���������� ��� ��������� ����������
	void Init()
	{
		// ���� ������ - ����� ���� ����� ���������� ������. �� � ��� ���� � ����.
		var camp = FindComponentInWorld<camp>();
        if (camp)
            target = FindComponentInWorld<camp>().target;

		// ��������� ������� ��������
		health = max_health;
		// ���������� ������������ ������ ������� ��������, ������� ������������� 100%
		health_bar_width = health_bar.GetWidth(0);
	}
	
	// update ���������� ������ ����
	void Update()
	{
		if (!target)
			return;

		// ����� ������� �� ������
		var target_rotation = MathLib.RotationFromDir((vec3)(target.WorldPosition - node.WorldPosition), vec3.UP);
		// ������������ ������� ������� � ������� ���� �� ������������ ����, ������� ����� �������� * �����
		target_rotation = MathLib.RotateTowards(node.GetWorldRotation(), target_rotation, rot_speed * Game.IFps);
		// ������������� ���������� �������
		node.SetWorldRotation(target_rotation);

		// ������� ���� ������ �� �������� * �����
		node.Translate(0, Game.IFps * move_speed, 0);
	}

	public void Kill()
	{
		// �����: ������� ����
		node.DeleteLater();
	}

	public void Damage(float v)
	{
		// ������� �����
		health -= v;
		
		// ���� ������ �� �������� - �������
		if (health <= MathLib.EPSILON)
			Kill();
		else
			// ������ ������ ������� ��������
			health_bar.SetWidth(0, health_bar_width * (health / max_health));
	}
}