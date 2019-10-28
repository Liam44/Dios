using Dios.Models;
using Xunit;

namespace DiosTest.Models
{
    public class FlatDTOTest
    {
        [Fact]
        public void FlatDTO_NoParameter()
        {
            // Arrange

            // Act
            FlatDTO flatDTO = new FlatDTO();

            // Assert
            Assert.Equal(-1, flatDTO.ID);
            Assert.Equal(-1, flatDTO.Floor);
            Assert.Empty(flatDTO.Number);
            Assert.Empty(flatDTO.EntryDoorCode);
            Assert.Equal(-1, flatDTO.AddressID);

            Assert.NotNull(flatDTO.Address);
            Assert.Equal(-1, flatDTO.Address.ID);
            Assert.Empty(flatDTO.Address.Street);
            Assert.Empty(flatDTO.Address.Number);
            Assert.Empty(flatDTO.Address.ZipCode);
            Assert.Empty(flatDTO.Address.Town);
            Assert.Empty(flatDTO.Address.Country);
            Assert.Empty(flatDTO.Address.Flats);
            Assert.Empty(flatDTO.Address.Hosts);

            Assert.NotNull(flatDTO.Parameters);
            Assert.Empty(flatDTO.Parameters);

        }

        [Fact]
        public void FlatDTO_Null()
        {
            // Arrange
            Flat flat = null;

            // Act
            FlatDTO flatDTO = new FlatDTO(flat);

            // Assert
            Assert.Equal(-1, flatDTO.ID);
            Assert.Equal(-1, flatDTO.Floor);
            Assert.Empty(flatDTO.Number);
            Assert.Empty(flatDTO.EntryDoorCode);
            Assert.Equal(-1, flatDTO.AddressID);

            Assert.NotNull(flatDTO.Address);
            Assert.Equal(-1, flatDTO.Address.ID);
            Assert.Empty(flatDTO.Address.Street);
            Assert.Empty(flatDTO.Address.Number);
            Assert.Empty(flatDTO.Address.ZipCode);
            Assert.Empty(flatDTO.Address.Town);
            Assert.Empty(flatDTO.Address.Country);
            Assert.Empty(flatDTO.Address.Flats);
            Assert.Empty(flatDTO.Address.Hosts);

            Assert.NotNull(flatDTO.Parameters);
            Assert.Empty(flatDTO.Parameters);
        }

        [Fact]
        public void FlatDTO_NotNull()
        {
            // Arrange
            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            int addressId = 2;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            // Act
            FlatDTO flatDTO = new FlatDTO(flat);

            // Assert
            Assert.Equal(flatId, flatDTO.ID);
            Assert.Equal(floor, flatDTO.Floor);
            Assert.Equal(number, flatDTO.Number);
            Assert.Equal(entryDoorCode, flatDTO.EntryDoorCode);
            Assert.Equal(addressId, flatDTO.AddressID);

            Assert.NotNull(flatDTO.Address);
            Assert.Equal(-1, flatDTO.Address.ID);
            Assert.Empty(flatDTO.Address.Street);
            Assert.Empty(flatDTO.Address.Number);
            Assert.Empty(flatDTO.Address.ZipCode);
            Assert.Empty(flatDTO.Address.Town);
            Assert.Empty(flatDTO.Address.Country);
            Assert.Empty(flatDTO.Address.Flats);
            Assert.Empty(flatDTO.Address.Hosts);

            Assert.NotNull(flatDTO.Parameters);
            Assert.Empty(flatDTO.Parameters);
        }
    }
}
