using System.Collections.Generic;
using System.Linq;

namespace AutoFakeToy
{
    #region Implementation of System Under Test

    public class FeedPersonService
    {
        private readonly IPersonService _personService;
        private readonly IFoodService _foodService;

        public FeedPersonService(IPersonService personService, IFoodService foodService)
        {
            _personService = personService;
            _foodService = foodService;
        }

        public string FeedPerson()
        {
            Person person = _personService.GetPerson();
            Food food = _foodService.GetFood();

            string foodNameLowerCase = food.Name.ToLower();
            if (person.FoodsEnjoyed.Contains(food.Name))
            {
                return $"{person.Name} says, 'I love {foodNameLowerCase}s! That {foodNameLowerCase} was delicious!'";
            }

            return $"{person.Name} says, 'Hmm, I've never tried a {foodNameLowerCase} before. I think I'll pass.'";
        }
    }

    #endregion

    #region Interfaces for dependencies

    public interface IFoodService
    {
        Food GetFood();
    }

    public interface IPersonService
    {
        Person GetPerson();
    }

    #endregion


    #region Business Domain Object classes used by the System Under Test

    public class Food
    {
        public Food(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class Person
    {
        public Person(string name, IEnumerable<string> foodsEnjoyed)
        {
            Name = name;
            FoodsEnjoyed = foodsEnjoyed.ToList();
        }

        public string Name { get; }
        public IReadOnlyCollection<string> FoodsEnjoyed { get; }
    }

    #endregion


}
