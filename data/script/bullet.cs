using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "e44c97c7e488c3d58346dc2b5b3e830fa189eda8")]
public class bullet : Component
{
	public float speed = 6.0f; // скорость пули
	public float damage = 30.0f; // урон пули
	public float distance = 50f; // максимальная дальность полета пули
	public float radius = 0.1f; // радиус пули

	public AssetLink splash; // ссылка на эффект при попадании

	private dvec3 init_pos; 
	void Init()
	{
		// сохраняем позицию в которой пуля появилась, чтобы считать сколько она пролетела
		init_pos = node.WorldPosition;
	}

	void Update()
	{
		// пуля летит вперед
		node.Translate(0, Game.IFps * speed, 0);

		// пуля проверяет не сбила ли она комара.
		// ТУДУ: передалать на поиск по отрезу current_pos - prev_pos, таккак при таком подходе пуля может проскочить сквозь комара

        List<Node> nodes = new List<Node>();
        // собираем все ноды в радиусе пули
		World.GetIntersection(new WorldBoundSphere(node.WorldPosition, radius), nodes);
        // ищем среди них комара
		foreach (Node n in nodes)
        {
            mosquitto m = GetComponentInParent<mosquitto>(n);
            if (m)
            {
				// если нашли, причиняем добро комару
				m.Damage(damage);
				// удаляем пулю
				node.DeleteLater();
				// спавним эффект попадания
				Node s = World.LoadNode(splash.Path);
				s.WorldTransform = node.WorldTransform;
            }
        }


		// если пуля улетела дальша максимальной дистанции - удаляем ее
        if (MathLib.Distance2(init_pos, node.WorldPosition) > distance * distance)
			node.DeleteLater(); 
    }
}