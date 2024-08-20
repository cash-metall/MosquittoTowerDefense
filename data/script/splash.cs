using System.Collections;
using System.Collections.Generic;
using Unigine;

// компонента, которая просто удаляем эффект взрыва через определенное время
[Component(PropertyGuid = "22f31dc6936f34ad57d115b242af1fbed8fab89d")]
public class splash : Component
{
	public float time = 1.0f; // время перед удалением
	private float timer = 0.0f;
	void Init()
	{
		timer = time;
	}
	
	void Update()
	{
		// вышло время? удаляем
		if (timer <= 0.0f) 
			node.DeleteLater();

		timer -= Game.IFps;
	}
}