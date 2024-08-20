using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unigine;

[Component(PropertyGuid = "3ec45a1f1a29b4adcbb6ec03cf38cbb7087b1ca1")]
public class tower : Component
{
	public float visibility_distance = 30; // дальность видимости для башни
	public float reload_time = 1; // время перезарядки

	public Node rotate_node; // эту ноду будем вращать
	public Node muzzle; // мушка. из этой ноды будем стрелять 

    public float rotation_speed = 60; // скорость поворота башни градусы/секунда
	public float damage = 60.0f; // урон, наносимый башней


	private mosquitto target;
	private WorldBoundSphere bs;
	private float timer = 0.0f;

	void Init()
	{
		bs = new WorldBoundSphere(node.WorldPosition, visibility_distance);
	}
	
	void Update()
	{
		if (target && !target.node) // если текущая цель умерла - забываем ее
		{
			target = null;
		}

		if (!target) // если нет цели - ищем ее
		{
			double distance = visibility_distance * visibility_distance;
            List<Node> nodes = new List<Node>();
            World.GetIntersection(bs, nodes); // делаем интерсешен по сфере
            foreach (Node n in nodes) // проверяем каждую ноду, не комар ли она
            {
                mosquitto m = GetComponentInParent<mosquitto>(n);
                if (m)
                {
					// среди комаров ищем ближаешего
					double l2 = MathLib.Distance2(n.WorldPosition, node.WorldPosition);
                    if (l2 < distance) 
                    {
						distance = l2;
						target = m;
                    }
                }
            }
        }

		// если есть цель
		if (target) 
		{
			// берем направление и расстояние до цели
			vec3 dir_to_target = (vec3)(target.node.WorldPosition - rotate_node.WorldPosition);
			float distance = dir_to_target.Length;
			dir_to_target.Normalize();

			// если направление пушки и направление до цели равны (ну... почти) стреляем!
			if (MathLib.Equals(dir_to_target, rotate_node.GetWorldDirection(MathLib.AXIS.Y), 0.01f))
			{
				// точно, таймер еще проверяем
				if (timer <= 0)
				{
					// наносим урон
					target.Damage(damage); 
					// рисуем лазерный луч
					Visualizer.RenderLine3D(rotate_node.WorldPosition, target.node.WorldPosition, vec4.BLUE, 0.1f);
					// взводим таймер перезарядки
					timer = reload_time;
				}
			}
			else
			{
				// если напрвление на цель не совпадает с нашим направлением - поворачиваем голову на цель
                var target_rotation = MathLib.RotationFromDir(dir_to_target, vec3.UP); // целевое вращение на комара
				// rotateTowards поворачивает от вращения 1 до вращения 2 на угол
				// вращение 1 - текущее вращение
				// вращение 2 - целевое вращение
				// угол = скорость вращение * время
                var new_rotation = MathLib.RotateTowards(rotate_node.GetWorldRotation(), target_rotation, rotation_speed * Game.IFps);

				// применяем вращение
                rotate_node.SetWorldRotation(new_rotation, true);
            }


        }

		if (timer > 0)
			timer -= Game.IFps;
	}
}