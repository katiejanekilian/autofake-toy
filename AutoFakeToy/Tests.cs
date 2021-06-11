using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AutoFakeToy
{
    public class Dependencies_configuration_mixed_between_ACallTo_and_Provide
    {
        private readonly ITestOutputHelper _xUnitOutput;

        public Dependencies_configuration_mixed_between_ACallTo_and_Provide(ITestOutputHelper xUnitOutput)
        {
            _xUnitOutput = xUnitOutput;
        }

        [Fact]
        public void ACallTo_first_and_Provide_second()
        {
            // FAILS.
            // Anything configured with A.CallTo() is lost as soon as Provide<T>() is called.

            using (var fake = new AutoFake())
            {
                // Arrange
                var testPerson = new Person("Alice", new[] { "Apple" });
                A.CallTo(() => fake.Resolve<IPersonService>().GetPerson())
                    .Returns(testPerson);

                fake.Provide<IFoodService>(new MockFoodService("Apple"));

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);
                // The above line produces the following output in the xUnit console:
                //
                //     IPersonService
                //         .ToString(): Faked AutoFakeToy.IPersonService
                //         .GetType().FullName: Castle.Proxies.ObjectProxy
                //
                //     IFoodService
                //         .ToString(): AutoFakeToy.MockFoodService
                //         .GetType().FullName: AutoFakeToy.MockFoodService
                //
                //     Person  (instance returned by IPersonService.GetPerson())
                //         .ToString(): Faked AutoFakeToy.Person
                //         .GetType().FullName: Castle.Proxies.PersonProxy
                //         .Name:
                //         .FoodsEnjoyed: {  }
                //
                // The instance of Person returned by fake.Resolve<IPersonService>().GetPerson() should be
                // the instance of testPerson created above. Instead, it returned a faked Person object.
                // For some reason, the call to fake.Provide<IFoodService>() caused AutoFake to lose track of
                // the configured behavior for IPersonService.GetPerson().
                //
                // For all other tests, IPersonService.GetPerson() returns the testPerson instance as configured.

                // Act
                string result = sut.FeedPerson();

                // Assert
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }

        [Fact]
        public void Provide_first_and_ACallTo_second()
        {
            // PASSES.
            // It is the same test as above, except with the two calls in the Arrange section swapped.
            // Anything configured with A.CallTo() works as long as it happens after any calls to Provide<T>().

            using (var fake = new AutoFake())
            {
                // Arrange
                fake.Provide<IFoodService>(new MockFoodService("Apple"));

                var testPerson = new Person("Alice", new[] { "Apple" });
                A.CallTo(() => fake.Resolve<IPersonService>().GetPerson())
                    .Returns(testPerson);

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);
                // The above line produces the following output in the xUnit console:
                //
                //     IPersonService
                //         .ToString(): Faked AutoFakeToy.IPersonService
                //         .GetType().FullName: Castle.Proxies.ObjectProxy
                //
                //     IFoodService
                //         .ToString(): AutoFakeToy.MockFoodService
                //         .GetType().FullName: AutoFakeToy.MockFoodService
                //
                //         Person(instance returned by IPersonService.GetPerson())
                //         .ToString(): AutoFakeToy.Person
                //         .GetType().FullName: AutoFakeToy.Person
                //         .Name: Alice
                //         .FoodsEnjoyed: { Apple }
                //
                // As expected, the instance of Person returned by fake.Resolve<IPersonService>().GetPerson() is
                // the instance of testPerson created above. The line that calls A.CallTo() to configure
                // IPersonService.GetPerson() was moved below the call to fake.Provide<IFoodService>()

                // Act
                string result = sut.FeedPerson();

                // Assert
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }
    }

    public class All_dependencies_created_via_ACallTo
    {
        private readonly ITestOutputHelper _xUnitOutput;

        public All_dependencies_created_via_ACallTo(ITestOutputHelper xUnitOutput)
        {
            _xUnitOutput = xUnitOutput;
        }

        [Fact]
        public void All_dependencies_are_fakes_configured_via_ACallTo()
        {
            // PASSES.
            // Included to demonstrate that A.CallTo() works fine if Provide<T>() is not called.

            using (var fake = new AutoFake())
            {
                // Arrange
                var testPerson = new Person("Alice", new[] { "Apple" });
                A.CallTo(() => fake.Resolve<IPersonService>().GetPerson())
                    .Returns(testPerson);

                A.CallTo(() => fake.Resolve<IFoodService>().GetFood())
                    .Returns(new Food("Apple"));

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);

                // Act
                string result = sut.FeedPerson();

                // Assert
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }

        [Fact]
        public void Dependency_order_does_not_matter_when_fakes_created_via_ACallTo()
        {
            // PASSES.
            // Same test as above, except with the dependencies configured in the opposite order.

            using (var fake = new AutoFake())
            {
                // Arrange
                var testFood = new Food("Apple");
                A.CallTo(() => fake.Resolve<IFoodService>().GetFood())
                    .Returns(testFood);

                var testPerson = new Person("Alice", new[] { "Apple" });
                A.CallTo(() => fake.Resolve<IPersonService>().GetPerson())
                    .Returns(testPerson);

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);

                // Act
                string result = sut.FeedPerson();

                // Assert
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }
    }


    public class All_dependencies_injected_via_Provide
    {
        private readonly ITestOutputHelper _xUnitOutput;

        public All_dependencies_injected_via_Provide(ITestOutputHelper xUnitOutput)
        {
            _xUnitOutput = xUnitOutput;
        }

        [Fact]
        public void All_dependencies_are_instances_injected_via_Provide()
        {
            // PASSES.
            // Included to demonstrate that Provide<T>() also works fine when used on its own.

            using (var fake = new AutoFake())
            {
                // Arrange
                fake.Provide<IPersonService>(new MockPersonService("Alice", new[] { "Apple" }));
                fake.Provide<IFoodService>(new MockFoodService("Apple"));

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);

                // Act
                string result = sut.FeedPerson();

                // Assert
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }

        [Fact]
        public void Dependency_order_does_not_matter_when_fakes_are_instances_injected_via_Provide()
        {
            // PASSES.
            // Same test as above, except with the dependencies configured in the opposite order.

            using (var fake = new AutoFake())
            {
                // Arrange
                fake.Provide<IFoodService>(new MockFoodService("Apple"));
                fake.Provide<IPersonService>(new MockPersonService("Alice", new[] { "Apple" }));

                var sut = fake.Resolve<FeedPersonService>();

                _xUnitOutput.InspectInjectedObjects(fake);

                // Act
                string result = sut.FeedPerson();

                // Assert
                _xUnitOutput.WriteLine($"IPersonService type: {fake.Resolve<IPersonService>()}");
                _xUnitOutput.WriteLine($"IFoodService type:   {fake.Resolve<IFoodService>()}");
                result.Should().Be("Alice says, 'I love apples! That apple was delicious!'");
            }
        }
    }

    public static class TestOutputHelperExtensions
    {
        public static void InspectInjectedObjects(this ITestOutputHelper output, AutoFake fake)
        {
            var personService = fake.Resolve<IPersonService>();
            var foodService = fake.Resolve<IFoodService>();

            Person person = personService.GetPerson();
            string foodsEnjoyed = "{ " + string.Join(", ", person.FoodsEnjoyed) + " }";

            output.WriteLine("IPersonService");
            output.WriteLine($"    .ToString(): {personService}");
            output.WriteLine($"    .GetType().FullName: {personService.GetType().FullName}");
            output.WriteLine("");
            output.WriteLine("IFoodService");
            output.WriteLine($"    .ToString(): {foodService}");
            output.WriteLine($"    .GetType().FullName: {foodService.GetType().FullName}");
            output.WriteLine("");
            output.WriteLine("Person  (instance returned by IPersonService.GetPerson())");
            output.WriteLine($"    .ToString(): {person}");
            output.WriteLine($"    .GetType().FullName: {person.GetType().FullName}");
            output.WriteLine($"    .Name: {person.Name}");
            output.WriteLine($"    .FoodsEnjoyed: {foodsEnjoyed}");
        }
    }
}