using System;

public class MonoFuncs : MonoSingleton<MonoFuncs>
{
    public event Action UpdateHandler;
    public event Action FixedUpdateHandler;

	private void Update()
	{
		UpdateHandler?.Invoke();
	}

	private void FixedUpdate()
	{
		FixedUpdateHandler?.Invoke();
	}
}
