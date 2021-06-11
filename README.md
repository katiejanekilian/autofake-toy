# Autofac.Extras.FakeItEasy bug demonstration

This repository demonstrates what appears to be a bug with the AutoFake class in Autofac.Extras.FakeItEasy.

## Summary

Using the AutoFake class in the Autofac.Extras.FakeItEasy namespace, calling Provide() appears to wipe out 
anything configured before using FakeItEasy's A.CallTo() methods. Using A.CallTo() after calling Provide&lt;T&gt;() works as expected.

* I can't find anything in the documentation stating that A.CallTo() cannot be called before Provide&lt;T&gt;().
* Likewise, I can't find anything suggesting Provide&lt;T&gt;() cannot be used with A.CallTo().
* It seems the order in which you configure unrelated dependencies shouldn't matter.

## Project Structure

**Classes.cs** contains the minimal code necessary to demonstrate this bug. It provides a system under test with two dependencies.

* The class `FeedPersonService` serves as the system under test.
* `IFoodService` and `IPersonService` are interfaces that are consumed as dependencies of `FeedPersonService`.
* `Food` and `Person` are business domain objects used by `FeedPersonService`, `IFoodService`, and `IPersonService`.

**MockServices.cs** provides mock implementations of `IFoodService` and `IPersonService`.

* `MockFoodService` implements `IFoodService`
* `MockPersonService` implements `IPersonService`
* Both classes are intended to be used in unit tests
* Instances of these classes are the implementations supplied when calling Provide()

**Tests.cs** provides tests demonstrating the bug.

* The classes `All_dependencies_created_via_ACallTo` and `All_dependencies_injected_via_Provide` each contain two tests 
  meant to demonstrate it is possible to configure the two dependencies in either order. 
* The class `Dependencies_configuration_mixed_between_ACallTo_and_Provide` contains two tests demonstrating that an error occurs 
  when using A.CallTo() followed by Provide(), but works when configured the other way around.
* The class `TestOutputHelperExtensions`, which provides an extension method, InspectInjectedObjects().
  This method is called in each test to inspect the objects returned by AutoFake.Resolve&lt;T&gt;() and display information 
  to the xUnit test output.

All tests call `InspectInjectedObjects()`, an extension method that uses the AutoFake instance to resolve the two injected services.

## Note about NuGet packages

I have used the same packages in this toy project as I used in the production app where I am experiencing this problem. 
I thought that might be a good idea just in case this is indeed a bug, and in case it interacts with any of the installed 
packages somehow. This was my reason for including xUnit and FluentAssertions.
