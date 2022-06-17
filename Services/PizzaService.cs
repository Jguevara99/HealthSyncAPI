using ContosoPizza.Models;
using System.Net;

namespace ContosoPizza.Services;

public static class PizzaService
{
    static List<Pizza> Pizzas { get; }
    static int nextId = 3;

    static PizzaService()
    {
        Pizzas = new List<Pizza>
        {
            new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
            new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true }
        };
    }

    public static List<Pizza> GetAll() => Pizzas;
    public static Pizza? Get(int id) => Pizzas.FirstOrDefault((p) => p.Id == id);
    public static Pizza Add(Pizza pizza)
    {
        pizza.Id = nextId++;
        Pizzas.Add(pizza);
        return pizza;
    }
    public static Pizza Delete(int id)
    {
        var pizza = Get(id);
        if (pizza is null)
            throw new HttpException($"The pizza with id: {id} doesn't exists!", HttpStatusCode.NotFound);

        Pizzas.Remove(pizza);
        return pizza;
    }

    public static Pizza Update(Pizza pizza)
    {
        var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
        if (index == -1)
            throw new HttpException($"The pizza with id: {pizza.Id} doesn't exists!", HttpStatusCode.NotFound);

        return Pizzas[index] = pizza;
    }
}