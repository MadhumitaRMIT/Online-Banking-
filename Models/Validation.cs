using SimpleHashing;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class Validation
    {

        // Val for String...
        public bool NameVal(String input)
        {

            String reg;
            reg = @"^[a-zA-Z][a-zA-Z]*$";

            bool flag = true;

            if (input.IndexOf(" ") > 0)
            {

                foreach (String names in input.Split(" "))
                {

                    if (names.Length != 0)
                    {
                        MatchCollection mc = Regex.Matches(names, reg);

                        if (mc.Count != 0)
                        {
                            flag = true;
                        }
                        else
                        {

                            return false;

                        }
                    }
                }
            }
            else
            {
                if (input.Length != 0)
                {
                    MatchCollection mc = Regex.Matches(input, reg);

                    if (mc.Count != 0)
                    {
                        flag = true;
                    }
                    else
                    {

                        return false;

                    }
                }
            }
            if (flag)
                return true;
            else
            {
                return false;
            }
        }


        // Val for number...
        public bool NumberVal(String input)
        {
            String reg;
            reg = @"^(\+?[0-9]{2,}?[0-9]{2,})$+";

            if (input.Length != 0)
            {
                MatchCollection mc = Regex.Matches(input, reg);

                if (mc.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }



        // Val for PostCode...
        public bool PstVal(String input)
        {
            String reg;
            reg = @"^[0-9]+[0-9]*$";

            if (input.Length != 0)
            {
                MatchCollection mc = Regex.Matches(input, reg);

                if (mc.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }


        // Conversion of Password...
        public bool PsdChk(String psdSh, String psdRw)
        {
            if (PBKDF2.Verify(psdSh, psdRw))
            {
                return true;
            }
            return false;
        }


        public String CreateHash(string input)
        {
            const int SALT_SIZE = 24; // size in bytes
            const int HASH_SIZE = 20; // size in bytes
            const int ITERATIONS = 50000; // number of pbkdf2 iterations

            // Generate a salt
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            provider.GetBytes(salt);

            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, ITERATIONS);
            return Convert.ToBase64String(pbkdf2.GetBytes(HASH_SIZE));
        }



        // Val for Customer Account Check...




        // Reset Cutomer counter:



    }
}
