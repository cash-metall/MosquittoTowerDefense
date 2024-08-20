using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5dc48f0b3fd1b214bd08c3af1c9b35a7aebe0f0d")]
public class spawner : Component
{
	// ��� ������� ��� ������ ���� �������� � ���������
    public struct MonsterPack
	{
		public int count; // ���������� ������ ������ ����
		public AssetLink reference; // ������ �� ���� ����� ����
        public vec2 distance; // ���-���� ���������� �� ������ ��� ������
		public vec2 height; // ���-���� ������ ��� ������

        public MonsterPack() // �# �� ����� � ��������� ��� �������������? 
		{
			count = 10;
			reference = new AssetLink();
			distance = new vec2(20,30);
			height = new vec2(0.5,5);
		}
	};
    public struct MonsterWave
	{
		public float duration; // ������������ "�����"
		public List<MonsterPack> monsters; // ������ ����� ������
		public MonsterWave() // �# �� ����� � ��������� ��� �������������? 
        {
			duration = 10;
			monsters = new List<MonsterPack>();
		}
    };

	public List<MonsterWave> waves;

	private int currnet_wave = 0; 
	private float timer = 0;

	void Init()
	{
		
	}

	dvec3 RandomPosition(vec2 distance, vec2 height)
	{
		double r = Game.GetRandomDouble(distance.x, distance.y); // ������
		float a = Game.GetRandomFloat(0, MathLib.PI2); // ����
		double h = Game.GetRandomDouble(height.x, height.y); // ������
		float s, c;
		MathLib.SinCos(a, out s, out c);

		dvec3 ret = new dvec3(s * r, c * r, h);
		return ret;
	}

	void SpawnWave()
	{
		foreach (MonsterPack m in waves[currnet_wave].monsters)
		{
			for (int i = 0; i < m.count; i++) 
			{
				Node n = World.LoadNode(m.reference.Path); // ��������� ����
				n.WorldPosition = RandomPosition(m.distance, m.height); // ������������� � ��������� �������
				// ����: �������� ���������� �����������? �� ������?
			}
		}
	}

	void Update()
	{
		if (timer <= 0) // ���� ����� �����
		{
			SpawnWave(); // ��������� ����� �����
			currnet_wave++; 
			if (waves.Count == currnet_wave) // ���� ����� ��������� - ������? ���� ��������� ������
				currnet_wave = 0;

			timer = waves[currnet_wave].duration; // ������� ����� ������
		}

		timer -= Game.IFps;

	}
}