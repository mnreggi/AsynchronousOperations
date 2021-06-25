using System;
using System.Globalization;

namespace Asynchronous_Operations.Services.FileExample
{
    public class People
    {
        public string Name  { get; set; }
        public string City  { get; set; }
        public int PositiveFeedback  { get; set; }
        public int NegativeFeedback  { get; set; }

        public static People FromCSV(string text)
        {
            // Split the comma separated values
            var segments = text.Split(',');

            // Remove unnecessary characters and spaces
            for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');

            People people;
            try
            {
                people = new People
                {
                    Name = segments[0],
                    City = segments[1],
                    PositiveFeedback = Convert.ToInt32(segments[2], CultureInfo.InvariantCulture),
                    NegativeFeedback = Convert.ToInt32(segments[3], CultureInfo.InvariantCulture)
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return people;
        }
    }
}