namespace AutoFakeToy
{
    public class MockFoodService : IFoodService
    {
        private readonly string _foodName;

        public MockFoodService(string foodName)
        {
            _foodName = foodName;
        }

        public Food GetFood()
        {
            return new Food(_foodName);
        }
    }

    public class MockPersonService : IPersonService
    {
        private readonly string _personName;
        private readonly string[] _foodsEnjoyed;

        public MockPersonService(string personName, string[] foodsEnjoyed)
        {
            _personName = personName;
            _foodsEnjoyed = foodsEnjoyed;
        }

        public Person GetPerson()
        {
            return new Person(_personName, _foodsEnjoyed);
        }
    }
}