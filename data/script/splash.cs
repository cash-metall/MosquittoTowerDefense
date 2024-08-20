using System.Collections;
using System.Collections.Generic;
using Unigine;

// ����������, ������� ������ ������� ������ ������ ����� ������������ �����
[Component(PropertyGuid = "22f31dc6936f34ad57d115b242af1fbed8fab89d")]
public class splash : Component
{
	public float time = 1.0f; // ����� ����� ���������
	private float timer = 0.0f;
	void Init()
	{
		timer = time;
	}
	
	void Update()
	{
		// ����� �����? �������
		if (timer <= 0.0f) 
			node.DeleteLater();

		timer -= Game.IFps;
	}
}