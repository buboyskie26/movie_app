using Domain;
using System;
using System.Threading.Tasks;

namespace Application.Repository.Service.SampOCP
{
    public interface IAddingBehavior
    {
        ShoppingCartItem SampAddingMethod(ISampAddingProp prop);
    }
}