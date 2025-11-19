using System.Text.RegularExpressions;

namespace CpfApi.Services
{
    public class CpfValidator
    {
        
        public string? NormalizeAndValidate(string? cpfOriginal)
        {
            if (string.IsNullOrWhiteSpace(cpfOriginal))
                return null;

            var cpf = Regex.Replace(cpfOriginal, "[^0-9]", "");

            if (cpf.Length != 11)
                return null;

            if (new string(cpf[0], 11) == cpf)
                return null;

            if (!CheckDigits(cpf))
                return null;

            return cpf; 
        }

        private bool CheckDigits(string cpf)
        {
         
            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
        }
    }
}
