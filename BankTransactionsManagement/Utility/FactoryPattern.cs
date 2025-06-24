public class FactoryPattern()
{
    public void DriverFunction()
    {
        ICar car = CarFactory.CreateCar(CarType.BMW);
        car.Drive();
        
    }
}

public interface ICar
{
    public void Drive();
    public void Stop();
}
public class BMW : ICar
{
    public void Drive()
    {
        Console.WriteLine("BMW is driving.");
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}
public class Mercedes : ICar
{
    public void Drive()
    {
        Console.WriteLine("Mercedes is driving.");
    }

    public void Stop()
    {
        Console.WriteLine("Mercedes has stopped.");
    }
}
public class CarFactory
{
    public static ICar CreateCar(CarType carType)
    {
        {
            return carType switch
            {
                CarType.BMW => new BMW(),
                CarType.Mercedes => new Mercedes(),
                _ => throw new ArgumentException("Invalid car type")
            };
        }
    }
}
public enum CarType
{
    BMW,
    Mercedes
}