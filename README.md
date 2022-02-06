# AutoFixture.AutoServiceCollection
Integration library between AutoFixture and .Net Core DI

## Project Description
Integrate AutoFixture with DI as sometimes, it might be needed. I've run into this problem when I was extending one of publicly available libraries that internally uses `_serviceProvider.GetService(serviceType)` and I had to create component tests. I wasn't able to easily, with low amount of code create what I intended to as I wanted to use AutoFixture, and to create instances of other objects manually. 

## Overview
This plugin was intended to lower the amount of code which has to be produced when we want to use AutoFixture to create some objects and still use what's defined in DI. Without duplication of definitions in AutoFixture it wasn't easily possible until now.

AutoFixture.AutoServiceCollection can help you by creating a connection between DI registrations and AutoFixture definitions. Whenever AutoFixture is about to create an object which registration is available in DI, it uses it. Whenever it's not, it uses its own internal logic for providing such objects.

Example:

```c#
public class MyClass 
{
	public readonly SingletonClass InnerObject;
	
	public MyClass(SingletonClass innerObject) 
	{
		InnerObject = innerObject;
	}
}

[Fact]
public void IntroductoryTest()
{
    Fixture fixture = new Fixture().Customize(new AutoServiceCollectionCustomization());
	var serviceCollection = fixture.Freeze<IServiceCollection>(); //it may be as well fixture.Create<IServiceCollection>()
	
	serviceCollection.AddSingleton(sp => new SingletonClass());
	
	var serviceProvider = fixture.Freeze<IServiceProvider>();
	
	var singletonInstance = serviceProvider.GetService<SingletonClass>();
	
	var sut = fixture.Create<MyClass>();
		
	sut.InnerObject.Should().BeSameAs(singletonInstance)
}
```

## Versioning

AutoFixture.AutoServiceCollection [Semantic Versioning 2.0.0](http://semver.org/spec/v2.0.0.html) for the public releases (published to the [nuget.org](https://www.nuget.org/)).

## Additional resources

* [Rafal Kozlowski's blog](https://rafalkozlowski.engineer)