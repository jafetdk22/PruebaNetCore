using System.Reflection;

namespace CompanyAPI.Services.SetOfNumber
{
    public class SetofNumberService: ISetofNumberService
    {
        private static List<int> numbers = Enumerable.Range(1, 100).ToList();

        public async Task<string> Extract(int number)
        {
            // Validación: El número debe ser menor o igual a 100
            if (number < 1 || number > 100)
            {
                throw new ArgumentOutOfRangeException("The number must be between 1 and 100.");
            }

            // Verificamos antes de eliminar
            Console.WriteLine($"Before removal: {string.Join(", ", numbers)}");

            // Eliminamos el número de la lista
            bool removed = numbers.Remove(number);

            if (removed)
            {
               return ($"Number {number} removed successfully.");
            }
            else
            {
                return ($"Number {number} not found in the list.");
            }
        }


        public async Task<int> CalculateMissingNumber()
        {
            // El número faltante será el único que no esté en la lista
            for (int i = 1; i <= 100; i++)
            {
                if (!numbers.Contains(i))
                {
                    return i;
                }
            }
            return -1; // Si no se encuentra, algo salió mal
        }
    }
}
