using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IFlatsRepository
    {
        IEnumerable<FlatDTO> Flats(int addressID);
        IEnumerable<FlatDTO> Flats(string userId);
        FlatDTO Flat(int? id);
        int Add(FlatDTO flat);
        int Edit(FlatDTO flat);
        int Delete(int flatId);
        bool Exists(int addressId, int? floor, string number);
        int AmountAvailableFlats(int addressId);
    }
}
