using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repository.Service.ShoppingProcessor.Interface
{
    public interface IAbstractFactory
    {
        IAbstractFactory CreateProductA();
        IAbstractFactory CreateProductB();
        bool AnotherUsefulFunctionB(IAbstractFactory productA);
    }
    public interface IAbstractProductB
    {
        // Product B is able to do its own thing...
        string UsefulFunctionB();

        // ...but it also can collaborate with the ProductA.
        //
        // The Abstract Factory makes sure that all products it creates are of
        // the same variant and thus, compatible.
        string AnotherUsefulFunctionB(IAbstractProductA collaborator);
    }

    public interface IAbstractProductA
    {
        string UsefulFunctionA();
    }

    public class Experiment
    {
        public void ClientMethod(IAbstractFactory factory)
        {
            var productA = factory.CreateProductA();
            var productB = factory.CreateProductB();

            /*Console.WriteLine(productB.UsefulFunctionB());*/
            Console.WriteLine(productB.AnotherUsefulFunctionB(productA));
        }

    }
}
