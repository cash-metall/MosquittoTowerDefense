using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c5f6f4ec4d2e7a1283220678a6aa9c6448cff312")]
public class mosquitto : Component
{
	public ObjectBillboards health_bar; // указатель на билбордик-полоску здоровья
	public float rot_speed = 30.0f; // скорость вращения в градусах в секунду
	public float move_speed = 1.0f; // скорость движения в метрах в секунду
	public float damage = 100.0f; // урон, который нанесет комар, добравшийся до лагеря
	public float max_health = 100.0f; // начальное здоровье комара

	private Node target; // текушая цель
	private float health_bar_width; // исходная ширина
	private float health; // текущее здоровье

	// инит вызывается при появлении компоненты
	void Init()
	{
		// ищем лагерь - место куда комар собирается лететь. он у нас один в мире.
		var camp = FindComponentInWorld<camp>();
        if (camp)
            target = FindComponentInWorld<camp>().target;

		// заполняем текущее здоровье
		health = max_health;
		// запоминаем максимальную ширину полоски здоровья, которая соответствует 100%
		health_bar_width = health_bar.GetWidth(0);
	}
	
	// update вызывается каждый кадр
	void Update()
	{
		if (!target)
			return;

		// берем поворот на лагерь
		var target_rotation = MathLib.RotationFromDir((vec3)(target.WorldPosition - node.WorldPosition), vec3.UP);
		// поворачиваем текущий поворот в сторону цели на фиксированый угол, который равен скорость * время
		target_rotation = MathLib.RotateTowards(node.GetWorldRotation(), target_rotation, rot_speed * Game.IFps);
		// устанавливаем полученный поворот
		node.SetWorldRotation(target_rotation);

		// двигаем ноду вперед на скорость * время
		node.Translate(0, Game.IFps * move_speed, 0);
	}

	public void Kill()
	{
		// убили: удаляем ноду
		node.DeleteLater();
	}

	public void Damage(float v)
	{
		// наносим ущерб
		health -= v;
		
		// если жизней не осталось - умираем
		if (health <= MathLib.EPSILON)
			Kill();
		else
			// меняем ширину полоски здоровья
			health_bar.SetWidth(0, health_bar_width * (health / max_health));
	}
}