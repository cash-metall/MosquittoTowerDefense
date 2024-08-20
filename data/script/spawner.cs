using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5dc48f0b3fd1b214bd08c3af1c9b35a7aebe0f0d")]
public class spawner : Component
{
	// тут примеры как разные типы выглядят в редакторе
    public struct MonsterPack
	{
		public int count; // количество юнитов одного типа
		public AssetLink reference; // ссылка на ноду этого типа
        public vec2 distance; // мин-макс расстояние от лагеря для спавна
		public vec2 height; // мин-макс высота для спавна

        public MonsterPack() // с# не умеет в структуры без конструкторов? 
		{
			count = 10;
			reference = new AssetLink();
			distance = new vec2(20,30);
			height = new vec2(0.5,5);
		}
	};
    public struct MonsterWave
	{
		public float duration; // длительность "волны"
		public List<MonsterPack> monsters; // список типов юнитов
		public MonsterWave() // с# не умеет в структуры без конструкторов? 
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
		double r = Game.GetRandomDouble(distance.x, distance.y); // радиус
		float a = Game.GetRandomFloat(0, MathLib.PI2); // угол
		double h = Game.GetRandomDouble(height.x, height.y); // высота
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
				Node n = World.LoadNode(m.reference.Path); // загружаем ноду
				n.WorldPosition = RandomPosition(m.distance, m.height); // устанавливаем в случайную позицию
				// ТУДУ: добавить ориентацию какуюнибудь? на лагерь?
			}
		}
	}

	void Update()
	{
		if (timer <= 0) // если время вышло
		{
			SpawnWave(); // запускаем новую волну
			currnet_wave++; 
			if (waves.Count == currnet_wave) // если волны кончились - победа? пока запускаем заново
				currnet_wave = 0;

			timer = waves[currnet_wave].duration; // взводим новый таймер
		}

		timer -= Game.IFps;

	}
}